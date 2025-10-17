seq 1 10

printf "%s\n" {1..10}

for i in {1..10}; do echo "$i"; done

# python3 -c 'print('\n'.join(str(i) for i in range(1,11)))'

i=1; while [ $i -le 10 ]; do echo $i; i=$((i + 1)); done
