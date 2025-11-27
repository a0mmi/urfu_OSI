#!/bin/bash

INPUT_FILE="users_list.txt"
REPORT="creation_report.txt"

> "$REPORT"  # Отчёт чищу

while IFS=: read -r username uid groupname comment home shell; do
  # Делаю группу если не существует
  getent group "$groupname" >/dev/null || groupadd "$groupname"
  
  # Создаю пользователя
  if useradd -u "$uid" -g "$groupname" -c "$comment" -d "$home" -s "$shell" -m "$username" 2>/dev/null; then
    # Пароль делаю (тут "123")
    echo "$username:123" | chpasswd
    echo "OK: $username создан" >> "$REPORT" # Ошибка на случай, если  не создаётся
  else
    echo "ERR: Ошибка создания $username" >> "$REPORT"
  fi
done < "$INPUT_FILE"

echo "Создание завершено. Отчет: $REPORT"

# Всё идёт в report