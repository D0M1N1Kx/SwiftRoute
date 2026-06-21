import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../providers/dashboard_provider.dart';
import '../Shared/order_list_tile.dart';
import '../Shared/section_header.dart';
import '../Shared/stat_card.dart';

class AdminDashboard extends ConsumerStatefulWidget {
  const AdminDashboard({super.key});

  @override
  ConsumerState<AdminDashboard> createState() => _AdminDashboardState();
}

class _AdminDashboardState extends ConsumerState<AdminDashboard> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref.read(dashboardProvider.notifier).loadAdminDashboard(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(dashboardProvider);

    if (state.isLoading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (state.error != null) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(Icons.error_outline, size: 48, color: Colors.red),
            const SizedBox(height: 12),
            Text(state.error!),
            const SizedBox(height: 12),
            FilledButton(
              onPressed: () =>
                  ref.read(dashboardProvider.notifier).loadAdminDashboard(),
              child: const Text('Retry'),
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: () =>
          ref.read(dashboardProvider.notifier).loadAdminDashboard(),
      child: SingleChildScrollView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Dashboard',
              style: Theme.of(
                context,
              ).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 24),

            // Stat kártyák
            GridView.count(
              crossAxisCount: _crossAxisCount(context),
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              crossAxisSpacing: 16,
              mainAxisSpacing: 16,
              childAspectRatio: 1.6,
              children: [
                StatCard(
                  icon: Icons.receipt_long_outlined,
                  label: 'Total Orders',
                  value: _totalOrders(state).toString(),
                  color: Colors.blue,
                  subtitle: '${state.orderStats['pending'] ?? 0} pending',
                ),
                StatCard(
                  icon: Icons.local_shipping_outlined,
                  label: 'In Transit',
                  value: (state.orderStats['in_transit'] ?? 0).toString(),
                  color: Colors.indigo,
                  subtitle: '${state.orderStats['assigned'] ?? 0} assigned',
                ),
                StatCard(
                  icon: Icons.check_circle_outlined,
                  label: 'Delivered',
                  value: (state.orderStats['delivered'] ?? 0).toString(),
                  color: Colors.green,
                ),
                StatCard(
                  icon: Icons.people_outlined,
                  label: 'Active Couriers',
                  value: state.activeCouriers.toString(),
                  color: Colors.orange,
                  subtitle: '${state.totalUsers} total users',
                ),
                StatCard(
                  icon: Icons.warehouse_outlined,
                  label: 'Warehouses',
                  value: state.totalWarehouses.toString(),
                  color: Colors.purple,
                ),
                StatCard(
                  icon: Icons.error_outline,
                  label: 'Failed / Cancelled',
                  value:
                      ((state.orderStats['failed'] ?? 0) +
                              (state.orderStats['cancelled'] ?? 0))
                          .toString(),
                  color: Colors.red,
                ),
              ],
            ),

            const SizedBox(height: 32),

            SectionHeader(
              title: 'Recent Orders',
              action: TextButton(
                onPressed: () {},
                child: const Text('View all'),
              ),
            ),
            const SizedBox(height: 12),

            Card(
              elevation: 0,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
                side: BorderSide(
                  color: Theme.of(context).colorScheme.outlineVariant,
                ),
              ),
              child: state.recentOrders.isEmpty
                  ? const Padding(
                      padding: EdgeInsets.all(24),
                      child: Center(child: Text('No orders yet')),
                    )
                  : ListView.separated(
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemCount: state.recentOrders.length,
                      separatorBuilder: (_, __) => const Divider(height: 1),
                      itemBuilder: (context, index) {
                        return OrderListTile(order: state.recentOrders[index]);
                      },
                    ),
            ),
          ],
        ),
      ),
    );
  }

  int _totalOrders(DashboardState state) {
    return state.orderStats.values.fold(0, (sum, count) => sum + count);
  }

  int _crossAxisCount(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    if (width > 1200) return 3;
    if (width > 800) return 2;
    return 1;
  }
}
