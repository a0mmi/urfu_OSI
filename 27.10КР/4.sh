#!/bin/bash

# cоздаю пользователя и тестовые файлы
useradd -m former_employee
echo "Test data 1" > /home/former_employee/file1.txt
echo "Test data 2" > /home/former_employee/file2.txt

# Блокирую учетную запись тремя способами
passwd -l former_employee  # Блокировка пароля
usermod -s /sbin/nologin former_employee  # Запрет shell
usermod -e "2025-01-01" former_employee  # Срок действия

# Архивирую домашнюю директорию
tar -czf /tmp/former_employee_backup.tar.gz /home/former_employee

# Удаляю учетную запись, сохраняя домашнюю директорию
userdel former_employee

# Проверка: домашняя директория должна остаться
echo "Архив создан: /tmp/former_employee_backup.tar.gz"
echo "Домашняя директория сохранена: /home/former_employee"