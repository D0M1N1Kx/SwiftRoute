import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';

class PackageStatusChart extends StatelessWidget {
  final Map<String, List<int>> series; // status -> values per day
  final List<String> labels; // same length as series values

  const PackageStatusChart({
    super.key,
    required this.series,
    required this.labels,
  });

  @override
  Widget build(BuildContext context) {
    if (labels.isEmpty || series.isEmpty) return const SizedBox();

    final days = labels.length;
    final statuses = series.keys.toList();
    // colors in stable order matching statuses used in provider
    final colorMap = {
      'Pending': Colors.lightBlue.shade200,
      'Assigned': Colors.purple.shade300,
      'PickedUp': Colors.orange.shade300,
      'InTransit': Colors.indigo.shade300,
      'Delivered': Colors.green.shade400,
      'Failed': Colors.red.shade300,
      'Cancelled': Colors.grey.shade400,
    };

    List<BarChartGroupData> groups = [];

    for (int i = 0; i < days; i++) {
      double start = 0;
      final rodStacks = <BarChartRodStackItem>[];
      for (final status in statuses) {
        final value = (series[status]![i]).toDouble();
        if (value <= 0) continue;
        rodStacks.add(
          BarChartRodStackItem(
            start,
            start + value,
            colorMap[status] ?? Colors.grey,
          ),
        );
        start += value;
      }

      final rod = BarChartRodData(
        toY: start,
        fromY: 0,
        rodStackItems: rodStacks,
        width: 18,
        borderRadius: BorderRadius.circular(6),
      );

      groups.add(BarChartGroupData(x: i, barRods: [rod]));
    }

    return SizedBox(
      height: 250,
      child: Card(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        child: Padding(
          padding: const EdgeInsets.all(12.0),
          child: BarChart(
            BarChartData(
              barGroups: groups,
              titlesData: FlTitlesData(
                leftTitles: AxisTitles(
                  sideTitles: SideTitles(showTitles: true),
                ),
                bottomTitles: AxisTitles(
                  sideTitles: SideTitles(
                    showTitles: true,
                    getTitlesWidget: (value, meta) {
                      final idx = value.toInt();
                      if (idx < 0 || idx >= labels.length)
                        return const SizedBox();
                      return Padding(
                        padding: const EdgeInsets.only(top: 6.0),
                        child: Text(
                          labels[idx],
                          style: const TextStyle(fontSize: 10),
                        ),
                      );
                    },
                    reservedSize: 36,
                  ),
                ),
              ),
              gridData: FlGridData(show: true),
              borderData: FlBorderData(show: false),
            ),
          ),
        ),
      ),
    );
  }
}
