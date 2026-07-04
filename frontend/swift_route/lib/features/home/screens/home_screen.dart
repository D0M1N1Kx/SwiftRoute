import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:swift_route/features/auth/providers/auth_provider.dart';
import 'package:swift_route/features/auth/screens/LoginScreen.dart';
import 'package:swift_route/features/dashboard/screens/dashboard_screen.dart';
import 'package:swift_route/features/home/widgets/side_nav.dart';

import '../models/NavItem.dart';
import '../widgets/bottom_nav.dart';

class HomeScreen extends ConsumerStatefulWidget {
  const HomeScreen({super.key});

  @override
  ConsumerState<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends ConsumerState<HomeScreen> {
  int _selectedIndex = 0;

  final List<NavItem> _navItems = [
    NavItem(icon: Icons.dashboard_outlined, label: 'Dashboard'),
    NavItem(icon: Icons.receipt_long_outlined, label: 'Orders'),
    NavItem(icon: Icons.map_outlined, label: 'Map'),
    NavItem(icon: Icons.warehouse_outlined, label: 'Warehouse'),
    NavItem(icon: Icons.people_outlined, label: 'Users'),
  ];

  final List<Widget> _screens = [
    const DashboardScreen(),
    const Center(child: Text('Orders')),
    const Center(child: Text('Map')),
    const Center(child: Text('Warehouses')),
    const Center(child: Text('Users')),
  ];

  void _logout() {
    ref.read(authProvider.notifier).logout();
    Navigator.of(
      context,
    ).pushReplacement(MaterialPageRoute(builder: (_) => const LoginScreen()));
  }

  @override
  Widget build(BuildContext context) {
    final authState = ref.watch(authProvider);
    final isDesktop = MediaQuery.of(context).size.width >= 600;

    if (authState.data == null) return const SizedBox();

    return Scaffold(
      body: isDesktop
          ? _buildDesktopLayout(authState)
          : _buildMobileLayout(authState),
    );
  }

  Widget _buildDesktopLayout(AuthState authState) {
    return Row(
      children: [
        SideNav(
          selectedIndex: _selectedIndex,
          items: _navItems,
          onDestinationSelected: (index) {
            setState(() => _selectedIndex = index);
          },
          authData: authState.data!,
          onLogout: _logout,
        ),
        const VerticalDivider(thickness: 1, width: 1),
        Expanded(child: _screens[_selectedIndex]),
      ],
    );
  }

  Widget _buildMobileLayout(AuthState authState) {
    return Scaffold(
      body: _screens[_selectedIndex],
      bottomNavigationBar: BottomNav(
        selectedIndex: _selectedIndex,
        items: _navItems,
        onDestinationSelected: (index) {
          setState(() => _selectedIndex = index);
        },
      ),
    );
  }
}
