set terminal pdf enhanced
set output 'pdf/results-lat0.pdf'
set datafile separator ","


set title "Response time per service (no latency)"
set style histogram errorbars gap 2 lw 1
set style data histogram
set ylabel 'Response time (ms)'
set xtics rotate by -45
set yrange [0:*]

plot for [i=2:7:2] "test.csv" every ::1 using i:i+1:xtic(1) title col(i)
