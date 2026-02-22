# 🍽️ 2D Restaurant Time-Management Game (Alpha)

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?style=flat-square&logo=unity)
![C#](https://img.shields.io/badge/C%23-Programming-blue?style=flat-square&logo=c-sharp)
![Architecture](https://img.shields.io/badge/Architecture-Event--Driven-green?style=flat-square)

## 📌 About the Project
This project is a 2D Time-Management simulation developed with Unity. My core focus was to design **interconnected game systems** (Customer AI, Order Management, Dishes) using clean architecture and high-performance patterns.

---

## 🎮 Gameplay Showcase

I've showcased the game in two different phases to demonstrate system stability and "Game Feel" under stress:

### 1. The Core Loop (Basic Gameplay)
Focuses on **Finite State Machine (FSM)** transitions and basic order-matching logic.
<div align="center">
  <video src="https://github.com/user-attachments/assets/e3a1a3c1-63a1-46ae-b73b-8e906c365049
" autoplay loop muted playsinline width="85%"></video>
</div>

### 2. The Rush Hour (Stress Test & Juice)
Demonstrates the **Object Pooling** system and **PrimeTween** animations (dynamic shaking, coin scattering) under high capacity.
<div align="center">
  <video src="https://github.com/user-attachments/assets/c2f3f7f0-8086-4415-8c7b-b8fb88199642
" autoplay loop muted playsinline width="85%"></video>
</div>

---

## 🚀 Technical Highlights
* **🧠 Custom FSM:** Modular AI logic for customer behaviors.
* **📡 Event-Driven Communication:** Decoupled systems using an **EventBus** pattern to maintain scalability.
* **♻️ Optimized Pooling:** Zero-allocation approach for high-frequency objects (Coins, Plates).
* **✨ Juice & Feedback:** Used **PrimeTween** for procedural animations, including a dynamic dishwashing station that shakes based on fill capacity.

---

## 🕹️ Play it on Itch.io
You can try the latest alpha build directly in your browser:
👉 **[Click Here to Play on Itch.io](BURAYA_ITCHIO_LINKINI_YAPISTIR)**

---

## 🛠️ Current Status & Roadmap
The project is currently in the **Alpha** stage. I am focusing on refining the core loop before expanding the meta-progression.

- [x] Core Gameplay Loop (Ordering, Cooking, Serving)
- [x] Dynamic Dishwashing & UI Juice
- [x] Basic Economy & Object Pooling
- [ ] **Coming Soon:** Kitchen Equipment Upgrades
- [ ] **Coming Soon:** Enhanced Level Design & Map Polish
- [ ] **Coming Soon:** Advanced Customer Types (VIPs, Impatient Critics)

---

## 📂 Other Projects
- 👻 **[3D Horror Game & Interactive Menu Design](LINK_BURAYA)**
- 🎨 **[Custom Shaders & Technical Art Portfolio](LINK_BURAYA)**

## 📬 Contact
[Your Name] - [LinkedIn](LINK_BURAYA) - [Email]
