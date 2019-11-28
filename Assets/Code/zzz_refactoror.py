import os


f = []
for (dirpath, dirnames, filenames) in os.walk("."):
    f.extend(filenames)
    break

for file in f:
	if file[-3:] == ".cs":
		print("Valid " + str(file))
		openFile = open(file,"r")
		lines = []
		for line in openFile.readlines():
			lines.append(line)
			print(line)