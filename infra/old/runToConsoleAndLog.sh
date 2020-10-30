# this can be used to output to the console and to a log file
# useful for debugging these scripts
./infra.azcli 2>&1 | tee ../secrets/logs/file.txt 