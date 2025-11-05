#!/bin/bash
# DÜZELTME: CourseApp API projesini durdurmak için script. Çalışan dotnet process'lerini bulup sonlandırıyor.
echo "CourseApp API projelerini durduruluyor..."

# DÜZELTME: CourseApp.API process'lerini bulup durduruyor. pkill komutu ile process adına göre arama yapılıyor.
pkill -f "CourseApp.API" || true

# DÜZELTME: dotnet run process'lerini de durduruyor. Eğer dotnet run ile çalıştırıldıysa onu da sonlandırıyor.
pkill -f "dotnet.*CourseApp.API" || true

echo "Proje durduruldu!"

