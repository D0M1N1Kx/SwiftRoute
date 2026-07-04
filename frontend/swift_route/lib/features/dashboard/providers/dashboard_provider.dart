import 'package:dio/dio.dart';
import 'package:flutter_riverpod/legacy.dart';
import 'package:swift_route/core/api/api_client.dart';
import 'package:swift_route/core/api/api_endpoints.dart';
import 'package:swift_route/core/models/order_model.dart';

import '../../../core/api/api_provider.dart';

class DashboardState {
  final bool isLoading;
  final String? error;
  final List<OrderModel> recentOrders;
  final Map<String, int> orderStats;
  final int totalUsers;
  final int activeCouriers;
  final int totalWarehouses;
  final Map<String, int> roleCounts;

  DashboardState({
    this.isLoading = false,
    this.error,
    this.recentOrders = const [],
    this.orderStats = const {},
    this.totalUsers = 0,
    this.activeCouriers = 0,
    this.totalWarehouses = 0,
    this.roleCounts = const {},
  });

  DashboardState copyWith({
    bool? isLoading,
    String? error,
    List<OrderModel>? recentOrders,
    Map<String, int>? orderStats,
    int? totalUsers,
    int? activeCouriers,
    int? totalWarehouses,
    Map<String, int>? roleCounts,
  }) {
    return DashboardState(
      isLoading: isLoading ?? this.isLoading,
      error: error ?? this.error,
      recentOrders: recentOrders ?? this.recentOrders,
      orderStats: orderStats ?? this.orderStats,
      totalUsers: totalUsers ?? this.totalUsers,
      activeCouriers: activeCouriers ?? this.activeCouriers,
      totalWarehouses: totalWarehouses ?? this.totalWarehouses,
      roleCounts: roleCounts ?? this.roleCounts,
    );
  }
}

class DashboardNotifier extends StateNotifier<DashboardState> {
  final ApiClient _apiClient;

  DashboardNotifier(this._apiClient) : super(DashboardState());

  Future<void> loadAdminDashboard() async {
    state = state.copyWith(isLoading: true, error: null);

    try {
      final results = await Future.wait([
        _apiClient.dio.get(ApiEndpoints.orders),
        _apiClient.dio.get(ApiEndpoints.users),
        _apiClient.dio.get(ApiEndpoints.warehouses),
        _apiClient.dio.get(ApiEndpoints.workerCountsByRole),
      ]);

      final orders = (results[0].data as List)
          .map((o) => OrderModel.fromJson(o))
          .toList();

      final users = results[1].data as List;
      final warehouses = results[2].data as List;
      final workerStatuses = results[3].data as List;

      final stats = <String, int>{};
      for (final order in orders) {
        stats[order.status] = (stats[order.status] ?? 0) + 1;
      }

      final roleCounts = <String, int>{};
      for (final item in workerStatuses) {
        roleCounts[item['role'] as String] = item['count'] as int;
      }

      final activeCouriers = users
          .where((u) => u['role'] == 'Courier' && u['isActive'] == true)
          .length;

      state = state.copyWith(
        isLoading: false,
        recentOrders: orders.take(10).toList(),
        orderStats: stats,
        totalUsers: users.length,
        activeCouriers: activeCouriers,
        totalWarehouses: warehouses.length,
        roleCounts: roleCounts,
      );
    } on DioException catch (e) {
      print('DIO ERROR: ${e.message}');
      print('RESPONSE: ${e.response?.data}');
      print('STATUS: ${e.response?.statusCode}');
      state = state.copyWith(
        isLoading: false,
        error: 'Failed to load dashboard data',
      );
    }
  }
}

final dashboardProvider =
    StateNotifierProvider<DashboardNotifier, DashboardState>((ref) {
      final apiClient = ref.watch(apiClientProvider);
      return DashboardNotifier(apiClient);
    });
