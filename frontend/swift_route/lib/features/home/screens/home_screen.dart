import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:swift_route/features/home/widgets/SideNav.dart';

import '../models/NavItem.dart';
import '../widgets/BottomNav.dart';

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
    const Center(child: Text('Dashboard')),
    const Center(child: Text('Orders')),
    const Center(child: Text('Map')),
    const Center(child: Text('Warehouses')),
    const Center(child: Text('Users')),
  ];

  @override
  Widget build(BuildContext context) {
    final isDesktop = MediaQuery.of(context).size.width >= 600;

    return Scaffold(
      body: isDesktop ? _buildDesktopLayout() : _buildMobileLayout(),
    );
  }

  Widget _buildDesktopLayout() {
    return Row(
      children: [
        SideNav(
            selectedIndex: _selectedIndex,
            items: _navItems,
            onDestinationSelected: (index) {
              setState(() => _selectedIndex = index);
            }),
        const VerticalDivider(thickness: 1, width: 1),
        Expanded(child: _screens[_selectedIndex])
      ],
    );
  }

  Widget _buildMobileLayout() {
    return Scaffold(
      body: _screens[_selectedIndex],
      bottomNavigationBar: BottomNav(
        selectedIndex: _selectedIndex,
        items: _navItems,
        onDestinationSelected: (index) {
          setState(() => _selectedIndex = index);
        }
      ),
    );
  }
}