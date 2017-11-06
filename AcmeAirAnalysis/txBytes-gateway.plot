set terminal pdf enhanced
set output 'pdf/txBytes-nginx1.pdf'
set datafile separator ","


set title "Sent bytes per service"
set style data histogram
set ylabel 'Bytes'
set format y '%.0s%cB'
set xtics rotate by -45
set yrange [0:*]
set key outside

plot for [COL=2:10:1] 'txBytes-gateway-2017-11-06-11.21.44.csv' using COL:xticlabels(1) title columnheader