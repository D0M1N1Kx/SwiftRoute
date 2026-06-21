class WarehouseModel {
  final String id;
  final String name;
  final String? address;
  final String? city;
  final double? latitude;
  final double? longitude;
  final bool isActive;

  WarehouseModel({
    required this.id,
    required this.name,
    this.address,
    this.city,
    this.latitude,
    this.longitude,
    required this.isActive,
  });

  factory WarehouseModel.fromJson(Map<String, dynamic> json) {
    return WarehouseModel(
      id: json['id'],
      name: json['name'],
      address: json['address'],
      city: json['city'],
      latitude: json['latitude']?.toDouble(),
      longitude: json['longitude']?.toDouble(),
      isActive: json['isActive'],
    );
  }
}
