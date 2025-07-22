from flask import Flask, request, jsonify
import google.generativeai as genai

app = Flask(__name__)
print("🔥 ai_api.py (Gemini FREE version - Full Log + Fallback) loaded. Route /suggest is active.")

# 🔑 Gemini API key
GEMINI_API_KEY = "AIzaSyBFSqZb7BU3Wn7m574FHtJmRf6nQk_gdGQ"  # <-- thay bằng key của bạn
genai.configure(api_key=GEMINI_API_KEY)

# ✅ Danh sách model (ưu tiên thử lần lượt)
MODEL_PRIORITY = [
    "models/gemini-1.5-flash",  # Miễn phí, nhanh
    "models/gemini-1.0-pro"     # Miễn phí (fallback)
]

@app.route("/", methods=["GET"])
def home():
    return "✅ Flask (Gemini FREE version - Full Log + Fallback) is working!"

@app.route("/suggest", methods=["POST"])
def suggest():
    data = request.get_json()
    print("📥 Dữ liệu nhận được từ request:", data)
    user_input = data.get("userInput", "") if data else ""

    if not user_input:
        return jsonify([]), 400

    suggestions = []
    last_error = None

    for model_name in MODEL_PRIORITY:
        try:
            print(f"🚀 Đang gọi Gemini với model: {model_name}")
            model = genai.GenerativeModel(model_name)
            prompt = (
                "Bạn là một chuyên gia ẩm thực Việt Nam. "
                "Chỉ trả về danh sách tên các món ăn phù hợp, mỗi món một dòng, không kèm giải thích. "
                f"Yêu cầu: {user_input}"
            )
            response = model.generate_content(prompt)

            # 🐞 Log chi tiết
            print(f"🔍 Raw response ({model_name}):", response)
            print(f"🔍 Text ({model_name}):", response.text)

            # Xử lý kết quả
            response_text = (response.text or "").strip()
            if response_text:
                suggestions = [
                    item.strip("-• \n") for item in response_text.split("\n") if item.strip()
                ]

            if suggestions:  # ✅ Nếu có kết quả thì thoát vòng lặp
                print(f"✅ Model {model_name} trả về {len(suggestions)} món ăn.")
                break
            else:
                print(f"⚠️ Model {model_name} không trả về kết quả hợp lệ, thử model khác...")

        except Exception as e:
            last_error = str(e)
            print(f"❌ Lỗi khi gọi Gemini với model {model_name}: {last_error}")
            continue  # Thử model tiếp theo

    if not suggestions:
        print("❌ Không có kết quả từ tất cả các model.")
        if last_error:
            print(f"⚠️ Lỗi cuối cùng: {last_error}")
        return jsonify([]), 500

    return jsonify(suggestions)

print("📋 Available routes:")
for rule in app.url_map.iter_rules():
    print(rule)

if __name__ == "__main__":
    print("🚀 AI server (Gemini FREE version - Full Log + Fallback) is running...")
    app.run(port=5000)
