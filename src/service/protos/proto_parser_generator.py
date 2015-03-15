# Copyright (c) 2011 Nohros Inc. All rights reserved.
# Use of this source code is governed by a MIT-style license that can be
# found in the LICENSE file.

# Call the .proto compiler for a specific language

import sys
import os
import subprocess

argv_size = len(sys.argv)
if argv_size != 3 and argv_size != 4:
  print("Usage protos_compiler.py PROTO_COMPILER_FILE_PATH [proto dir] [proto extension]")
  sys.exit(0)
try: 
  proto_compiler_file_path = sys.argv[1]
  directory = sys.argv[2]

  if argv_size == 4:
    extension = sys.argv[3]
  else:
    extension = ".proto"

  # Find all *.extension files with filter
  files = [file for file in os.listdir(directory) if file.endswith(extension)]

  print ("Nohros Inc. Protocol Buffer compiler invocation")
  print ("")
  print ("Compiling *.protos using: ")
  print ("  Compiler: " + proto_compiler_file_path)
  print ("  Proto(s) path: " + directory)
  print ("  Extension: " + extension)
  print ("  Amount: " + str(len(files)))
  print ("")
  
  # compile all the proto specified proto files
  for file in files:
    script = "\"" + proto_compiler_file_path + "\" " + directory + "\\" + file
    print ("Executing the compiler script")
    print ("  " + script)
    os.system(script)
    
except Exception as inst:
  print (inst)
  sys.exit(1)