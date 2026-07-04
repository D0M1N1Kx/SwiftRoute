import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../providers/dashboard_provider.dart';
import '../../../core/widgets/stat_card.dart';

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
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Padding(
              padding: const EdgeInsets.only(top: 16.0),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.center,
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Dashboard',
                        style: Theme.of(context).textTheme.headlineSmall
                            ?.copyWith(fontWeight: FontWeight.bold),
                      ),
                      Text(
                        'Welcome back, USERNAME!',
                        style: Theme.of(context).textTheme.labelMedium,
                      ),
                    ],
                  ),
                  Icon(Icons.account_circle, color: Colors.blue, size: 36.0),
                ],
              ),
            ),
            Divider(color: Colors.grey[400], thickness: 1),

            // Stat kártyák
            Text("Overview", style: TextStyle(fontWeight: FontWeight.bold)),
            _buildOverview(state),

            const SizedBox(height: 16),

            /*_buildRolesStats(state),*/
            const SizedBox(height: 16),
            _buildWorkerStats(state),
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

Widget _buildOverview(DashboardState state) {
  return GridView.count(
    crossAxisCount: 2,
    shrinkWrap: true,
    physics: const NeverScrollableScrollPhysics(),
    padding: EdgeInsets.only(top: 8.0),
    crossAxisSpacing: 12,
    mainAxisSpacing: 12,
    childAspectRatio: 2,
    children: [
      StatCard(
        title: "Users",
        value: state.totalUsers.toString(),
        icon: Icons.people,
        color: Colors.blue,
        subtitle: "Active: ${state.activeCouriers}",
      ),
      StatCard(
        title: "Active Couriers",
        value: state.activeCouriers.toString(),
        icon: Icons.delivery_dining,
        color: Colors.green,
      ),
      StatCard(
        title: "Warehouses",
        value: state.totalWarehouses.toString(),
        icon: Icons.warehouse,
        color: Colors.purple,
        subtitle: "All",
      ),
      StatCard(
        title: "Total Orders",
        value: state.orderStats.values
            .fold(0, (sum, count) => sum + count)
            .toString(),
        icon: Icons.inventory,
        color: Colors.orange,
        subtitle: "All warehouses",
      ),
    ],
  );
}

Widget _buildWorkerStats(DashboardState state) {
  return Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      const Text(
        "Workers by roles",
        style: TextStyle(fontWeight: FontWeight.bold),
      ),
      const SizedBox(height: 12),
      Wrap(
        spacing: 10,
        runSpacing: 10,
        children: state.orderStats.entries.map((e) {
          return StatCard(
            title: e.key,
            value: e.value.toString(),
            icon: Icons.circle,
            color: _statusColor(e.key),
          );
        }).toList(),
      ),
    ],
  );
} // TODO: NEED TO BE FIXED LIKE ON THE IMAGE

Color _statusColor(String status) {
  switch (status) {
    case 'Pending':
      return Colors.grey;
    case 'Assigned':
      return Colors.blue;
    case 'PickedUp':
      return Colors.orange;
    case 'InTransit':
      return Colors.indigo;
    case 'Delivered':
      return Colors.green;
    case 'Failed':
      return Colors.red;
    case 'Cancelled':
      return Colors.black54;
    default:
      return Colors.grey;
  }
}
