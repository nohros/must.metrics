# Copyright (c) 2011 Nohros Inc. All rights reserved.
# Use of this source code is governed by a MIT-style license that can be
# found in the LICENSE file.

# Call the .proto compiler for a the csharp language using the protobuf-csharp-port
# as the proto compiler. This script was created to be called by double clicking it
# on windows machines.

import sys
import os
import subprocess

root = "."

# Get the subfolders path
subdirs = [subdir for subdir in os.listdir(root) if os.path.isdir(os.path.join(root, subdir))]
subdirs.append('.')
for subdir in subdirs:
  if os.path.exists(os.path.join(subdir, 'proto.makefile')):
   script = 'proto_parser_generator.py "protogen.exe --include_imports -output_directory=./' + os.path.join(subdir, 'parsers/csharp/') + ' --proto_path=. --proto_path=./'+ subdir + '" ./' + subdir +' .proto'
   os.system(script)
os.system('pause')