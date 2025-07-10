# TranslatorApp

A lightweight WPF desktop application that translates between **All Languages** using a local Python Flask API with HuggingFace Transformer (Facebook M2M100 model).

---

## 📁 Project Structure

```
TranslatorApp/
├── TranslatorApp.sln
├── WpfApp/                  # WPF .NET Desktop UI (C#)
├── backend/                 # Python Flask API + Model
│   ├── translate_api.py
│   ├── download_model.py
│   └── model/ (ignored in Git)
├── .gitignore
└── README.md
```

---

## 🚀 Features

✅ Translate between Arabic, Persian, and English.  
✅ Offline mode – model runs locally.  
✅ Auto-starts the Python API in background when WPF launches.  
✅ Saves source/target language preferences.  
✅ Supports right-to-left text alignment based on language.  
✅ Copy input/output buttons.  
✅ Reverse translation button (🔁 Swap languages).

---

## 🧪 How to Run Locally

### ✅ Requirements
- Python 3.9+ installed
- .NET 6 or 7 SDK (for WPF)

---

### 🔧 Backend Setup (Python API)

1. Navigate to the `backend/` folder:
   ```bash
   cd backend
   ```

2. Create virtual environment:
   ```bash
   python -m venv venv
   venv\Scripts\activate   # On Windows
   ```

3. Install dependencies:
   ```bash
   pip install -r requirements.txt
   ```

4. (Optional) Test it manually:
   ```bash
   python translate_api.py
   ```

---

### ⚙️ Frontend Setup (WPF)

1. Open `TranslatorApp.sln` in Visual Studio.
2. Set `WpfApp` as startup project.
3. Build and run.

ℹ️ On launch, the WPF app will automatically start `pythonw.exe backend/translate_api.py` in background.

---

## 📦 How to Package (Offline)

- You can bundle the `venv`, `model`, and `translate_api.py` inside your installer.
- Make sure `pythonw.exe` is properly referenced in the code (`ProcessStartInfo`).

---

## 🛑 Notes

- The model folder (`model/`) is large, and **should not be pushed to GitHub**.
- Add it to `.gitignore` (already configured).
- You can optionally convert the backend API to `.exe` using `pyinstaller`.

---

## ✅ Example API Request (via Postman)

```
POST http://127.0.0.1:5000/translate
Content-Type: application/json

{
  "source": "سلام",
  "source_lang": "fa",
  "target_lang": "en"
}
```

Response:
```json
{
  "translated_text": "Hello"
}
```

---

## 🙌 Credits

- [Facebook/M2M100-418M](https://huggingface.co/facebook/m2m100_418M)
- [Helsinki-NLP/opus-mt](https://huggingface.co/Helsinki-NLP)
- [Flask](https://flask.palletsprojects.com/)
- [HuggingFace Transformers](https://huggingface.co/docs/transformers/)

---

## 📃 License
MIT License © 2025 Majid Gharaee
