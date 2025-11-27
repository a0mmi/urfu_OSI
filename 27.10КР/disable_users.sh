#!/bin/bash

INPUT_FILE="users_to_disable.txt"

# Проверка наличия файла, т.к. в прошлом гарантированно существовал файл, а тут не факт
[ ! -f "$INPUT_FILE" ] && echo "Ошибка: $INPUT_FILE не найден" && exit 1

while read -r username; do
  # Блокаю тремя способами
  passwd -l "$username" &>/dev/null
  usermod -s /sbin/nologin "$username" &>/dev/null
  usermod -e "1970-01-01" "$username" &>/dev/null
  
  echo "Заблокирован: $username"
done < "$INPUT_FILE"
