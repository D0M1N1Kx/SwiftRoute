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

  DashboardState({
    this.isLoading = false,
    this.error,
    this.recentOrders = const [],
    this.orderStats = const {},
    this.totalUsers = 0,
    this.activeCouriers = 0,
    this.totalWarehouses = 0,
  });

  DashboardState copyWith({
    bool? isLoading,
    String? error,
    List<OrderModel>? recentOrders,
    Map<String, int>? orderStats,
    int? totalUsers,
    int? activeCouriers,
    int? totalWarehouses,
  }) {
    return DashboardState(
      isLoading: isLoading ?? this.isLoading,
      error: error ?? this.error,
      recentOrders: recentOrders ?? this.recentOrders,
      orderStats: orderStats ?? this.orderStats,
      totalUsers: totalUsers ?? this.totalUsers,
      activeCouriers: activeCouriers ?? this.activeCouriers,
      totalWarehouses: totalWarehouses ?? this.totalWarehouses,
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
      ]);

      final orders = (results[0].data as List)
          .map((o) => OrderModel.fromJson(o))
          .toList();

      final users = results[1].data as List;
      final warehouses = results[2].data as List;

      final stats = <String, int>{};
      for (final order in orders) {
        stats[order.status] = (stats[order.status] ?? 0) + 1;
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
      );
    } on DioException catch (e) {
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
