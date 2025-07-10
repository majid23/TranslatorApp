from transformers import M2M100ForConditionalGeneration, M2M100Tokenizer

model_name = "facebook/m2m100_418M"
local_dir = "./model"  # پوشه‌ای که کنار translate_api.py باشه

# دانلود مدل و توکنایزر به‌صورت آفلاین
M2M100Tokenizer.from_pretrained(model_name, cache_dir=local_dir)
M2M100ForConditionalGeneration.from_pretrained(model_name, cache_dir=local_dir)