set terminal pdf enhanced
set output 'pdf/rxPackets.pdf'
set datafile separator ","


set title "Received packets per service"
set style data histogram
set ylabel 'Packets'
set format y '%.0s%c'
set xtics rotate by -45
set yrange [0:*]
set key outside

plot for [COL=2:10:1] 'rxPackets-2017-11-06-11.21.44.csv' using COL:xticlabels(1) title columnheader