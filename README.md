# 🍽️ 2D Restaurant Time-Management Game (Alpha Build)

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?style=flat-square&logo=unity)
![C#](https://img.shields.io/badge/C%23-Programming-blue?style=flat-square&logo=c-sharp)
![Architecture](https://img.shields.io/badge/Architecture-Event--Driven-green?style=flat-square)
![Animation](https://img.shields.io/badge/Animation-PrimeTween-orange?style=flat-square)

## 📌 Project Overview
This project is a 2D Time-Management simulation developed in Unity. The primary objective was to engineer a suite of **interconnected game systems** (Customer AI, Order Management, Dishwashing Cycles) using a clean, modular architecture. It is highly optimized for **Web/WebGL platforms**, ensuring high performance and a low memory footprint. The project is currently in the **MVP (Minimum Viable Product)** stage and remains under active development.

---

## 🎮 Gameplay Showcase

The following clips demonstrate the system's stability and "Game Feel" during both standard operation and high-intensity scenarios:

### 1. The Core Loop (Basic Mechanics)
Focuses on seamless **Finite State Machine (FSM)** transitions, customer interaction logic, and procedural order matching.
<div align="center">
  <video src="https://github.com/user-attachments/assets/e3a1a3c1-63a1-46ae-b73b-8e906c365049" autoplay loop muted playsinline width="85%"></video>
</div>

### 2. The Rush Hour (Stress Test & Juice)
Demonstrates the reliability of the **Object Pooling** system and the impact of **PrimeTween**-powered procedural animations (dynamic station shaking, coin scattering) during high-traffic gameplay.
<div align="center">
  <video src="https://github.com/user-attachments/assets/847e0c37-e848-47af-a49e-bc2156e29d98" autoplay loop muted playsinline width="85%"></video>
</div>

---

## 🚀 Technical Highlights

* **🧠 Custom Finite State Machine (FSM):** Implemented a modular AI for customer behaviors (Walking, Waiting for Menu, Eating, Leaving), allowing for easy extension of new customer types.
* **📡 Event-Driven Communication:** Leveraged a custom **EventBus** pattern to decouple systems. UI updates and game logic communicate via events, reducing dependencies and preventing "spaghetti code."
* **♻️ Optimized Object Pooling:** Utilized a **Zero-Allocation** approach for high-frequency objects (Coins, Plates, Customers) to significantly reduce Garbage Collector (GC) pressure.
* **✨ Procedural Animations (Juice):** Integrated **PrimeTween** for high-performance animations. Features include a dynamic dishwashing station that reacts to fill capacity and optimized ease-curves for UI feedback.
* **📦 Modularity & MVC:** Maintained strict separation between visual controllers (`TableVisualController`) and logical controllers (`TableController`) for better maintainability.
* **📂 Addressables System:** Implemented Unity **Addressables** for asynchronous asset loading, leading to faster initial load times and efficient RAM management—crucial for WebGL performance.
* **⚙️ ScriptableObject Architecture:** Utilized **ScriptableObjects** to create a data-driven system for daily schedules and customer varieties. This ensures the system is easily scalable, highly manageable, and decoupled from core logic.

---

## 🕹️ Live Demo (Play on Itch.io)
Experience the latest Alpha build directly in your browser:
👉 **[Play on Itch.io](https://efesmsk05.itch.io/denemepixcoffe)**

---

## 🛠️ Roadmap & Current Status
The project has successfully reached its Alpha milestone with core mechanics fully functional.

- [x] Core Gameplay Loop (Ordering, Cooking, Serving)
- [x] Dynamic Dishwashing System & Visual Feedback
- [x] Economy System & Optimized Object Pooling
- [x] Addressables Integration for WebGL
- [ ] **Upcoming:** Kitchen Equipment Upgrade System
- [ ] **Upcoming:** Advanced Level Design & Map Polish
- [ ] **Upcoming:** Dynamic Customer Types (VIPs, Impatient Critics)
- [ ] **Upcoming:** Custom Vector Art Overhaul & Aesthetic Polish

---

## 📝 Developer Notes
* **Project Status:** The game has recently transitioned out of the MVP stage. While the core loop is fully functional and stable, minor bugs may exist as the project scales.
* **Assets:** Current visual assets are placeholders (free assets). A comprehensive art pass, including custom vector graphics and stylized shaders, is currently in progress.
* **Vision:** This project is being developed with the goal of publishing on web platforms like **CrazyGames** and **Poki**.
* **Purpose:** This repository serves to showcase the Alpha build for community testing and to document the implementation of modular game systems.

** project deneme notu : BootStrap shanesini başlatarak oyunu unityde deneyenbilrtsnz

---

## 📂 Other Technical Works
* 👻 **[3D Horror Game: Interactive Menus & Custom Shaders](BURAYA_3D_OYUNUNUN_LINKINI_EKLE)**

## 📬 Contact
* **LinkedIn:** [Efe Şimşek](https://www.linkedin.com/in/efe-%C5%9Fim%C5%9Fek-b41619356/)
* **Email:** [efesimsek0535@gmail.com]
