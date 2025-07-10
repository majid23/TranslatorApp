# translate_api.py
from flask import Flask, request, jsonify
from transformers import M2M100ForConditionalGeneration, M2M100Tokenizer
import torch

app = Flask(__name__)

# offline model
#local_dir = "./model/models--facebook--m2m100_418M/snapshots/55c2e61bbf05dfb8d7abccdc3fae6fc8512fd636"
#model = M2M100ForConditionalGeneration.from_pretrained(local_dir)
#tokenizer = M2M100Tokenizer.from_pretrained(local_dir)

# online model
model_name = "facebook/m2m100_418M"
model = M2M100ForConditionalGeneration.from_pretrained(model_name)
tokenizer = M2M100Tokenizer.from_pretrained(model_name)

@app.route("/translate", methods=["POST"])
def translate():
    data = request.json
    src = data["source"]
    tgt_lang = data["target_lang"]
    src_lang = data["source_lang"]

    tokenizer.src_lang = src_lang
    encoded = tokenizer(src, return_tensors="pt")
    generated_tokens = model.generate(**encoded, forced_bos_token_id=tokenizer.get_lang_id(tgt_lang))
    translated = tokenizer.batch_decode(generated_tokens, skip_special_tokens=True)
    return jsonify({"translated_text": translated[0]})

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)
