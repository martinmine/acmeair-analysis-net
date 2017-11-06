set terminal pdf enhanced
set output 'pdf/cpuUsage.pdf'
set datafile separator ","

set title "CPU usage per service"
set style histogram errorbars gap 2 lw 1
set style data histogram
set ylabel 'CPU usage (%)'
set xtics rotate by -45
set yrange [0:*]
set key outside

plot for [i=2:18:2] "usage-2017-11-06-15.52.15.csv" every ::1 using i:i+1:xtic(1) title col(i)
