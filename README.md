# 🍽️ 2D Restaurant Time-Management Game (Alpha Build)

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?style=flat-square&logo=unity)
![C#](https://img.shields.io/badge/C%23-Programming-blue?style=flat-square&logo=c-sharp)
![Architecture](https://img.shields.io/badge/Architecture-Event--Driven-green?style=flat-square)
![Animation](https://img.shields.io/badge/Animation-PrimeTween-orange?style=flat-square)

## 📌 Proje Hakkında (About the Project)
Bu proje, Unity motoruyla geliştirilmiş 2D bir Zaman Yönetimi (Time-Management) simülasyonudur. Projenin ana odağı; birbirine bağlı oyun sistemlerinin (**Müşteri AI, Sipariş Yönetimi, Bulaşık Döngüsü**) temiz bir mimari, modüler yapı ve yüksek performanslı tasarım desenleri ile nasıl kurgulanabileceğini göstermektir.

---

## 🎮 Oynanış Gösterimi (Gameplay Showcase)

Sistemin hem temel işleyişini hem de yoğun yük altındaki performansını sergilemek adına iki farklı oynanış kesiti aşağıda sunulmuştur:

### 1. Temel Döngü (The Core Loop)
Bu bölümde **Finite State Machine (FSM)** geçişleri, müşteri etkileşimleri ve temel sipariş eşleştirme mantığı sergilenmektedir.
<div align="center">
  <video src="https://github.com/user-attachments/assets/e3a1a3c1-63a1-46ae-b73b-8e906c365049" autoplay loop muted playsinline width="85%"></video>
</div>

### 2. Yoğun Saat & Performans (The Rush Hour)
Restoranın tam kapasite çalıştığı bu anlarda; **Object Pooling** sisteminin kararlılığı ve **PrimeTween** ile yazılmış prosedürel animasyonların (dinamik titremeler, para saçılma efektleri) yarattığı "Game Feel" test edilmektedir.
<div align="center">
  <video src="https://github.com/user-attachments/assets/847e0c37-e848-47af-a49e-bc2156e29d98" autoplay loop muted playsinline width="85%"></video>
</div>

---

## 🚀 Teknik Öne Çıkanlar (Technical Highlights)

* **🧠 Custom Finite State Machine (FSM):** Müşteri davranışları (Yürüme, Menü Bekleme, Yemek Yeme, Terk Etme) için modüler ve genişletilebilir bir yapay zeka mantığı kuruldu.
* **📡 Event-Driven Communication:** Sistemler arasındaki bağımlılığı (coupling) azaltmak için **EventBus** yapısı kullanıldı. UI güncellemeleri ve müşteri aksiyonları bu hat üzerinden haberleşmektedir.
* **♻️ Optimized Object Pooling:** Sık oluşturulan nesneler (Paralar, Tabaklar, Müşteriler) için bellek dostu, "Zero-Allocation" yaklaşımı uygulandı.
* **✨ Prosedürel Animasyonlar (Juice):** Yüksek performanslı **PrimeTween** kütüphanesi entegre edildi. Bulaşık istasyonunun doluluk oranına göre dinamik olarak titremesi ve coinlerin fizik temelli saçılma efektleri gibi detaylarla "Game Feel" maksimize edildi.
* **📦 Modüler Mimari:** Görsel kontrolcüler (`TableVisualController`) ve mantıksal kontrolcüler (`TableController`) birbirinden ayrılarak kodun okunabilirliği ve bakımı kolaylaştırıldı.

---

## 🕹️ Hemen Dene (Play on Itch.io)
Oyunun en güncel Alpha sürümünü doğrudan tarayıcınız üzerinden deneyimleyebilirsiniz:
👉 **[Oyunun Itch.io Sayfası](https://efesmsk05.itch.io/denemepixcoffe)**

---

## 🛠️ Mevcut Durum ve Yol Haritası (Roadmap)
Proje şu an **Alpha** aşamasındadır ve çekirdek mekanikler başarıyla tamamlanmıştır.

- [x] Temel Oynanış Döngüsü (Sipariş, Pişirme, Servis)
- [x] Dinamik Bulaşık Sistemi ve UI Geri Bildirimleri
- [x] Ekonomi Sistemi ve Object Pooling Entegrasyonu
- [ ] **Yakında:** Mutfak Ekipmanları Geliştirme (Upgrade) Sistemi
- [ ] **Yakında:** Level Design ve Harita Cilalama
- [ ] **Yakında:** Özel Müşteri Tipleri (VIP, Sabırsız Eleştirmenler vb.)

---

## 📂 Diğer Çalışmalar (Other Projects)
* 👻 **[3D Korku Oyunu & İnteraktif Menü Tasarımı](BURAYA_3D_OYUN_REPOSUNUN_LINKINI_YAPISTIR)**
* 🎨 **[Teknik Sanat & Shader Çalışmaları](BURAYA_SHADER_KLASORUNUN_VEYA_REPOSUNUN_LINKINI_YAPISTIR)**

## 📬 İletişim (Contact)
* **LinkedIn:** [https://www.linkedin.com/in/efe-%C5%9Fim%C5%9Fek-b41619356/]
* **E-posta:** [efesimsek0535@gmail.com]
