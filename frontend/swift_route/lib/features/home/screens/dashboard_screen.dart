import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:swift_route/features/auth/providers/auth_provider.dart';
import 'package:swift_route/features/home/widgets/Dashboard/admin_dashboard.dart';

class DashboardScreen extends ConsumerWidget {
  const DashboardScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final authState = ref.watch(authProvider);
    final role = authState.data?.user.role ?? '';

    return switch (role) {
      'Admin' => const AdminDashboard(),
      'Dispatcher' => const Center(child: Text('Dispatcher Dashboard')),
      'Courier' => const Center(child: Text('Courier Dashboard')),
      'WarehouseStaff' => const Center(
        child: Text('Warehouse Staff Dashboard'),
      ),
      _ => const Center(child: Text('Dashboard')),
    };
  }
}
