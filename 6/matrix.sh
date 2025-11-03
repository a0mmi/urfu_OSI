#!/bin/bash

# Установка цвета: зелёный на чёрном фоне
echo -e "\e[40m\e[32m"
clear

# Получаем размеры терминала
lines=$(tput lines)
cols=$(tput cols)

# Массив символов БЕЗ обратной кавычки (`), чтобы избежать синтаксических ошибок
chars=("0" "1" "2" "3" "4" "5" "6" "7" "8" "9"
       "A" "B" "C" "D" "E" "F" "G" "H" "I" "J" "K" "L" "M" "N" "O" "P" "Q" "R" "S" "T" "U" "V" "W" "X" "Y" "Z"
       "a" "b" "c" "d" "e" "f" "g" "h" "i" "j" "k" "l" "m" "n" "o" "p" "q" "r" "s" "t" "u" "v" "w" "x" "y" "z"
       "!" "@" "#" "$" "%" "^" "&" "*" "(" ")" "-" "_" "=" "+" "[" "]" "{" "}" ";" ":" "'" "\"" "," "." "<" ">" "/" "?" "\\" "|" "~")

# Инициализация массивов
declare -a positions
declare -a lengths

# Инициализируем начальные позиции и длины "столбцов"
for ((i=0; i<cols; i++)); do
    positions[i]=-1
    lengths[i]=$((RANDOM % 20 + 5))
done

# Основной цикл
while true; do
    echo -ne "\e[H"  # Переместить курсор в верхний левый угол

    for ((i=0; i<cols; i++)); do
        if ((positions[i] == -1)) || ((positions[i] > lines)); then
            if ((RANDOM % 10 == 0)); then
                positions[i]=0
                lengths[i]=$((RANDOM % 20 + 5))
            fi
            continue
        fi

        # Рисуем хвост
        for ((j=0; j<lengths[i]; j++)); do
            y=$((positions[i] - j))
            if ((y >= 0 && y < lines)); then
                x=$((i + 1))
                char=${chars[RANDOM % ${#chars[@]}]}

                if ((j == 0)); then
                    echo -ne "\e[${y};${x}H\e[32m${char}"
                else
                    echo -ne "\e[${y};${x}H\e[2m${char}"
                fi
            fi
        done

        # Стираем последний символ
        last_y=$((positions[i] - lengths[i]))
        if ((last_y >= 0)); then
            echo -ne "\e[${last_y};$((i+1))H "
        fi

        ((positions[i]++))
    done

    sleep 0.05
done