#!/usr/bin/env python3

from itertools import permutations
from statistics import mean


if __name__ == '__main__':
    with open('data.txt') as hfile:
        data = list(hfile)

    current_match = []
    counts = []
    wait_times = []
    match_counts = []

    for line in data:
        (th_id, count, wait_time) = line.strip().split()
        if th_id == '1':
            match_counts.append(current_match)
            current_match = []

        current_match.append(int(count))
        counts.append(int(count))
        wait_times.append(int(wait_time))

    match_counts.append(current_match)
    match_counts.pop(0)

    print("count av:", int(mean(counts)))
    print("wait av:", int(mean(wait_times)))

    global_diffs = [
        [abs(x - y) for (x, y) in permutations(item, 2)]
        for item in match_counts
    ]

    global_av_diffs = []
    global_max_diff = []

    for global_diff in global_diffs:
        global_av_diffs.append(int(mean(global_diff)))
        global_max_diff.append(max(global_diff))

    print("average diff:", int(mean(global_av_diffs)))
    print("max diff:", max(global_max_diff))
