set terminal pdf enhanced
set output 'pdf/results-lat.pdf'
set datafile separator ","


set title "Response time per service"
set style histogram errorbars gap 2 lw 1
set style data histogram
set ylabel 'Response time (ms)'
set xtics rotate by -45
set yrange [0:*]
set key outside

plot for [i=2:18:2] "results-2017-11-03-15.47.44.csv" every ::1 using i:i+1:xtic(1) title col(i)
