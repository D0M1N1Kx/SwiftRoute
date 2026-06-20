import 'package:flutter/material.dart';

import '../models/NavItem.dart';

class SideNav extends StatelessWidget {
  final int selectedIndex;
  final List<NavItem> items;
  final ValueChanged<int> onDestinationSelected;

  const SideNav({
    super.key,
    required this.selectedIndex,
    required this.items,
    required this.onDestinationSelected
  });

  @override
  Widget build(BuildContext context) {
    return NavigationRail(
      selectedIndex: selectedIndex,
      onDestinationSelected: onDestinationSelected,
      extended: MediaQuery.of(context).size.width >= 900,
      leading: Padding(
        padding: const EdgeInsets.symmetric(vertical: 16),
        child: Column(
          children: [
            const Icon(Icons.local_shipping_outlined, size: 32),
            const SizedBox(height: 4),
            if (MediaQuery.of(context).size.width >= 900)
              const Text(
                'RouteXY',
                style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
              ),
          ],
        ),
      ),
      destinations: items.map((item) {
        return NavigationRailDestination(
          icon: Icon(item.icon),
          label: Text(item.label)
        );
      }).toList(),
    );
  }
}