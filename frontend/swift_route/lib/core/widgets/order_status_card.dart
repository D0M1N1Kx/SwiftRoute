import 'package:flutter/material.dart';

class OrderStatusCard extends StatelessWidget {
  final String orderStatus;
  final IconData iconData;

  const OrderStatusCard({
    Key? key,
    required this.orderStatus,
    required this.iconData,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.all(8.0),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Row(
          children: [
            Icon(iconData, size: 36.0),
            const SizedBox(width: 16.0),
            Text(
              orderStatus,
              style: const TextStyle(
                fontSize: 18.0,
                fontWeight: FontWeight.bold,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
