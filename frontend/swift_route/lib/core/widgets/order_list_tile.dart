import 'package:flutter/material.dart';

import '../models/order_model.dart';

class OrderListTile extends StatelessWidget {
  final OrderModel order;
  final VoidCallback? onTap;

  const OrderListTile({super.key, required this.order, this.onTap});

  @override
  Widget build(BuildContext context) {
    return ListTile(
      onTap: onTap,
      contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      leading: Container(
        padding: const EdgeInsets.all(8),
        decoration: BoxDecoration(
          color: _statusColor(order.status).withOpacity(0.1),
          borderRadius: BorderRadius.circular(8),
        ),
        child: Icon(
          _statusIcon(order.status),
          color: _statusColor(order.status),
          size: 18,
        ),
      ),
      title: Text(
        order.trackingNumber,
        style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 14),
      ),
      subtitle: Text(
        order.recipientName,
        style: TextStyle(
          fontSize: 12,
          color: Theme.of(context).colorScheme.onSurfaceVariant,
        ),
      ),
      trailing: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        crossAxisAlignment: CrossAxisAlignment.end,
        children: [
          _StatusChip(status: order.status),
          const SizedBox(height: 4),
          Text(
            _formatDate(order.createdAt),
            style: TextStyle(
              fontSize: 11,
              color: Theme.of(context).colorScheme.onSurfaceVariant,
            ),
          ),
        ],
      ),
    );
  }

  Color _statusColor(String status) {
    return switch (status.toLowerCase()) {
      'pending' => Colors.orange,
      'assigned' => Colors.blue,
      'picked_up' => Colors.purple,
      'in_transit' => Colors.indigo,
      'delivered' => Colors.green,
      'failed' => Colors.red,
      'cancelled' => Colors.grey,
      _ => Colors.grey,
    };
  }

  IconData _statusIcon(String status) {
    return switch (status.toLowerCase()) {
      'pending' => Icons.hourglass_empty_outlined,
      'assigned' => Icons.person_outlined,
      'picked_up' => Icons.inventory_2_outlined,
      'in_transit' => Icons.local_shipping_outlined,
      'delivered' => Icons.check_circle_outlined,
      'failed' => Icons.error_outlined,
      'cancelled' => Icons.cancel_outlined,
      _ => Icons.help_outline,
    };
  }

  String _formatDate(DateTime date) {
    return '${date.day}.${date.month.toString().padLeft(2, '0')}.${date.year}';
  }
}

class _StatusChip extends StatelessWidget {
  final String status;

  const _StatusChip({required this.status});

  @override
  Widget build(BuildContext context) {
    final color = _color();
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(20),
      ),
      child: Text(
        status.replaceAll('_', ' '),
        style: TextStyle(
          fontSize: 11,
          color: color,
          fontWeight: FontWeight.w500,
        ),
      ),
    );
  }

  Color _color() {
    return switch (status.toLowerCase()) {
      'pending' => Colors.orange,
      'assigned' => Colors.blue,
      'picked_up' => Colors.purple,
      'in_transit' => Colors.indigo,
      'delivered' => Colors.green,
      'failed' => Colors.red,
      'cancelled' => Colors.grey,
      _ => Colors.grey,
    };
  }
}
