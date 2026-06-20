import 'package:flutter/material.dart';

import '../models/NavItem.dart';

class BottomNav extends StatelessWidget {
  final int selectedIndex;
  final List<NavItem> items;
  final ValueChanged<int> onDestinationSelected;

  const BottomNav({
    super.key,
    required this.selectedIndex,
    required this.items,
    required this.onDestinationSelected
  });

  @override
  Widget build(BuildContext context) {
    return NavigationBar(
      selectedIndex: selectedIndex,
      onDestinationSelected: onDestinationSelected,
      destinations: items.map((item) {
        return NavigationDestination(
          icon: Icon(item.icon),
          label: item.label,
        );
      }).toList(),
    );
  }
}