import os 
import time 
import platform 
from datetime import datetime 

# ================= 配置区 ================= 
# 1. 你的 Unity 项目绝对路径
UNITY_PROJECT_PATH = r"d:\unity_projects\TypeRogue"

# 2. Markdown 文件的输出路径 (建议放在项目根目录)
MD_OUTPUT_DIR = UNITY_PROJECT_PATH
MD_FILE_NAME = "Cursor_UnityLogs.md"

# 3. 日志保留数量 (保留最新的N条日志)
MAX_LOG_ENTRIES = 30
# ========================================== 

def get_editor_log_path():
    """获取系统级的 Unity Editor.log 路径"""
    system = platform.system()
    if system == "Windows":
        return os.path.expandvars(r"%LOCALAPPDATA%\Unity\Editor\Editor.log")
    elif system == "Darwin": # macOS
        return os.path.expanduser("~/Library/Logs/Unity/Editor.log")
    else: # Linux
        return os.path.expanduser("~/.config/unity3d/Editor.log")

def get_project_name():
    """从项目路径获取项目名称"""
    return os.path.basename(UNITY_PROJECT_PATH.rstrip(os.sep))

def init_md_file(md_path):
    """初始化 Markdown 文件并写入标题"""
    os.makedirs(os.path.dirname(md_path), exist_ok=True)
    project_name = get_project_name()
    
    with open(md_path, 'w', encoding='utf-8') as f:
        f.write(f"# 🔴 Unity 实时报错日志 (供 Cursor 读取)\n\n")
        f.write(f"**项目:** {project_name}\n")
        f.write(f"**监控启动时间:** {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n\n")
        f.write("---\n\n")
    
    print(f"✅ Markdown 日志文件已就绪: {md_path}")

def parse_unity_log_line(line):
    """解析 Unity 日志行，提取关键信息"""
    line_lower = line.lower()
    
    # 检查是否包含错误或警告关键词
    has_error = any(keyword in line_lower for keyword in ['error', 'exception', 'failed', 'crash'])
    has_warning = 'warning' in line_lower and not has_error
    
    return {
        'has_error': has_error,
        'has_warning': has_warning,
        'is_compilation_error': 'error' in line_lower and any(code in line for code in ['CS', 'BC', 'UE', 'SH']),
        'line': line
    }

def format_log_entry(parsed_line, timestamp):
    """格式化单条日志为 Markdown"""
    if parsed_line['has_error']:
        error_type = "编译错误" if parsed_line['is_compilation_error'] else "运行时错误"
        return f"### ⏰ [{timestamp}] {error_type}\n**报错信息:**\n```text\n{parsed_line['line']}\n```\n---\n"
    elif parsed_line['has_warning']:
        return f"### ⏰ [{timestamp}] Warning\n**警告信息:**\n```text\n{parsed_line['line']}\n```\n---\n"
    else:
        return None  # 只记录错误和警告

def monitor_and_write():
    log_path = get_editor_log_path()
    md_path = os.path.join(MD_OUTPUT_DIR, MD_FILE_NAME)
    
    if not os.path.exists(log_path):
        print(f"❌ 找不到 Unity Editor.log 文件: {log_path}")
        print(f"💡 提示: 请确保 Unity 编辑器正在运行")
        return
    
    init_md_file(md_path)
    print(f"🚀 开始实时监控...")
    print(f"📋 监控的日志文件: {log_path}")
    print(f"📝 输出到 Markdown 文件: {md_path}")
    print(f"📊 保留最新 {MAX_LOG_ENTRIES} 条日志\n")
    print("💡 按 Ctrl+C 停止监控\n")

    # 存储最近的日志条目
    recent_logs = []

    # 打开系统日志 (读取) 和 MD 文件 (写入)
    with open(log_path, 'r', encoding='utf-8', errors='ignore') as log_file:
        
        # 将指针移动到 Editor.log 末尾，只监听最新产生的日志
        log_file.seek(0, 2)
        
        try:
            while True:
                line = log_file.readline()
                if not line:
                    time.sleep(0.1)  # 降低 CPU 占用
                    continue
                
                line_str = line.strip()
                if not line_str:
                    continue

                # 过滤掉一些 Editor 内部的无用信息
                line_lower = line_str.lower()
                skip_keywords = [
                    "refreshing native plugins",
                    "vfsing",
                    "assetdatabase.refresh",
                    "loading scene",
                    "unloading unused assets",
                    "shader compiler",
                    "compiling assembly"
                ]
                
                if any(keyword in line_lower for keyword in skip_keywords):
                    continue

                # 解析日志行
                parsed_line = parse_unity_log_line(line_str)
                
                # 只记录错误和警告
                if parsed_line['has_error'] or parsed_line['has_warning']:
                    timestamp = datetime.now().strftime('%H:%M:%S')
                    log_entry = format_log_entry(parsed_line, timestamp)
                    
                    if log_entry:
                        recent_logs.append(log_entry)
                        
                        # 限制日志数量
                        if len(recent_logs) > MAX_LOG_ENTRIES:
                            recent_logs.pop(0)
                        
                        # 重新写入整个文件
                        project_name = get_project_name()
                        with open(md_path, 'w', encoding='utf-8') as md_file:
                            md_file.write(f"# 🔴 Unity 实时报错日志 (供 Cursor 读取)\n\n")
                            md_file.write(f"**项目:** {project_name}\n")
                            md_file.write(f"**最后更新:** {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n\n")
                            md_file.write("---\n\n")
                            
                            if not recent_logs:
                                md_file.write("*暂无报错。*\n")
                            else:
                                md_file.write(''.join(recent_logs))
                        
                        # 打印控制台输出
                        if parsed_line['has_error']:
                            print(f"🔴 [{timestamp}] 错误: {line_str[:50]}...")
                        elif parsed_line['has_warning']:
                            print(f"🟡 [{timestamp}] 警告: {line_str[:50]}...")

        except KeyboardInterrupt:
            print("\n🛑 监控已手动停止。")
            print(f"📊 共捕获 {len(recent_logs)} 条日志")
        except Exception as e:
            print(f"\n❌ 监控过程中发生错误: {e}")

if __name__ == "__main__":
    try:
        monitor_and_write()
    except Exception as e:
        print(f"❌ 启动失败: {e}")
        input("按回车键退出...")