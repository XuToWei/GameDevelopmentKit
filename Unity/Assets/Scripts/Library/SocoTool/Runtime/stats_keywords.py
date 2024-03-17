#用于统计bundle中当前shader的变体
#使用方法：
#1. 用asset studio打开asset bundle
#2. 在asset list中找到shader
#3. 将shader复制保存到文本中
#4. 编写此脚本ShaderVariants.from_file输入文本路径

import re

ParseState = {}
ParseState['None'] = 0
ParseState['SubProgram'] = 1
ParseState['Pass'] = 2
ParseState['PassFind'] = 3
ParseState['Keywords'] = 4
ParseState['Local Keywords'] = 5

class ShaderVariants:

    def __init(self):
        pass

    #total_variants： dict key-pass, value-list of variants

    @staticmethod
    def from_file(filename):

        sv = ShaderVariants()
        sv.total_variants = {}

        with open(filename, 'r') as f:

            current_state = ParseState['None']
            current_keywords = []
            current_pass = ""

            for line in f:
                
                if current_state == ParseState['None']:
                    if len(re.findall('Pass \{', line)) > 0:
                        current_state = ParseState['Pass']

                elif current_state == ParseState['Pass']:
                    current_pass = re.findall('Name "(\w+)"', line)[0]
                    sv.total_variants[current_pass] = set()
                    current_state = ParseState['PassFind']

                elif current_state == ParseState['PassFind']:
                    if len(re.findall('SubProgram "gles3 " \{', line)) > 0:
                        current_state = ParseState['SubProgram']
                        current_keywords = []
                    elif len(re.findall('Pass \{', line)) > 0:
                        current_state = ParseState['Pass']

                elif current_state == ParseState['SubProgram']:
                    if line.startswith("Keywords"):
                        current_state = ParseState['Keywords']
                        current_keywords = re.findall('"(\w+)"', line)
                    elif line.startswith("Local Keywords"):
                        current_state = ParseState['Local Keywords']
                        current_keywords += re.findall('"(\w+)"', line)
                    else:
                        current_state = ParseState['PassFind']
                        sv.total_variants[current_pass].add(" ".join(current_keywords))

                elif current_state == ParseState['Keywords']:
                    if line.startswith("Local Keywords"):
                        current_state = ParseState['Local Keywords']
                        current_keywords += re.findall('"(\w+)"', line)
                    else:
                        current_state = ParseState['PassFind']
                        sv.total_variants[current_pass].add(" ".join(current_keywords))

                elif current_state == ParseState['Local Keywords']:
                    current_state = ParseState['PassFind']
                    sv.total_variants[current_pass].add(" ".join(current_keywords))

        return sv

# filename = "PFLitTreeOri.shader"
# filename2 = "PFLitTreeSoco.shader"

# sv1 = ShaderVariants.from_file(filename)
# sv2 = ShaderVariants.from_file(filename2)

# for k, v in sv1.total_variants.items():
#     print(f'pass:{k}, len:{len(v)}')

# print('------')
# for k, v in sv2.total_variants.items():
#     print(f'pass:{k}, len:{len(v)}')

# var1=sv1.total_variants['ForwardLit']
# var2=sv2.total_variants['ForwardLit']

# var1 = dict.fromkeys(var1)

# for k in var2:
#     if var1.get(k, None) == None:
#         print(k)

filename = "PFLitSoco.shader"
sv1 = ShaderVariants.from_file(filename)

for k, v in sv1.total_variants.items():
    for i in v:
        print(i)

    print('---------')