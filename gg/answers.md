Здравствуйте! кб-102.
По поводу сегодняшней контрольной.
Шухов Сергей и Максим Мокин.
1.

```
@echo off & set /a c=0 & for /f "delims=" %a in ('type "in.txt" ^| sort') do @(set /a c+=1 & if !c! leq 10 echo %a)
```

```
for /f "usebackq tokens=1* delims=:" %a in ("students.txt") do @for /f "tokens=1 delims= " %c in ("%b") do @if %c geq 10 echo %a
```

```
echo Шухова ыадя Евгеньевич | findstr /r /i "^.*ова .*адя .*вич$"
```

> (Проверка findstr на echo)
> Столкнулись с проблемой кодировки. .txt файлы в utf-8 или utf-16 записываются и при чтении ничего не находит, т.к. ищет в кодировке cp1251. Как поменять не додумались. (только с русскими символами проблема)
> Для файла:
> findstr /r /i "^.*ова .*адя .*вич$" "names.txt"

```
findstr /i "^bad" users.txt
```

```
for /L %i in (1,1,10) do @echo %i
for %i in (1 2 3 4 5 6 7 8 9 10) do @echo %i
echo 1&echo 2&echo 3&echo 4&echo 5&echo 6&echo 7&echo 8&echo 9&echo 10
```
