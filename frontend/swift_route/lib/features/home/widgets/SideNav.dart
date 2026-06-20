import 'dart:ffi';

import 'package:flutter/material.dart';
import '../../../core/models/auth_model.dart';
import '../models/NavItem.dart';

class SideNav extends StatelessWidget {
  final int selectedIndex;
  final List<NavItem> items;
  final ValueChanged<int> onDestinationSelected;
  final AuthModel authData;
  final VoidCallback onLogout;

  const SideNav({
    super.key,
    required this.selectedIndex,
    required this.items,
    required this.onDestinationSelected,
    required this.authData,
    required this.onLogout,
  });

  @override
  Widget build(BuildContext context) {
    final isExtended = MediaQuery.of(context).size.width >= 900;

    return NavigationRail(
      selectedIndex: selectedIndex,
      onDestinationSelected: onDestinationSelected,
      extended: isExtended,
      leading: Padding(
        padding: const EdgeInsets.symmetric(vertical: 16),
        child: Column(
          children: [
            const Icon(Icons.local_shipping_outlined, size: 32),
            const SizedBox(height: 4),
            if (isExtended)
              const Text(
                'RouteXY',
                style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
              ),
          ],
        ),
      ),
      trailing: SizedBox(
        width: isExtended ? 220 : 72,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Divider(),
            if (isExtended)
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 12),
                child: Row(
                  children: [
                    CircleAvatar(
                      child: Text(authData.user.fullName[0].toUpperCase()),
                    ),
                    const SizedBox(width: 10),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          authData.user.fullName,
                          style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13),
                          overflow: TextOverflow.ellipsis,
                        ),
                        Text(
                          authData.user.role,
                          style: const TextStyle(fontSize: 12, color: Colors.grey),
                        ),
                      ],
                    ),
                    const Spacer(),
                    IconButton(
                      icon: const Icon(Icons.logout_outlined),
                      tooltip: 'Logout',
                      onPressed: onLogout,
                    ),
                  ],
                ),
              )
            else
              Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  CircleAvatar(
                    child: Text(authData.user.fullName[0].toUpperCase()),
                  ),
                  const SizedBox(height: 8),
                  IconButton(
                    icon: const Icon(Icons.logout_outlined),
                    tooltip: 'Logout',
                    onPressed: onLogout,
                  ),
                ],
              ),
          ],
        ),
      ),
      destinations: items.map((item) {
        return NavigationRailDestination(
          icon: Icon(item.icon),
          label: Text(item.label),
        );
      }).toList(),
    );
  }
}