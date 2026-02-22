# 🍽️ 2D Restaurant Time-Management Game (Alpha)

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?style=flat-square&logo=unity)
![C#](https://img.shields.io/badge/C%23-Programming-blue?style=flat-square&logo=c-sharp)
![Architecture](https://img.shields.io/badge/Architecture-Event--Driven-green?style=flat-square)

<div align="center">
  <video src="SENIN_MP4_LINKIN_BURAYA" autoplay loop muted playsinline width="80%"></video>
</div>

## 📌 About the Project
This project is a 2D Time-Management simulation game developed with Unity and C#. My main goal was to design interconnected game systems (Customer AI, Order Management, Kitchen & Dishes) with clean architecture and highly optimized performance.

## 🚀 Technical Highlights & Features
As a developer, I focused heavily on scalable architecture and "Game Feel" (Juice). Here are the core technical implementations:

* **🧠 Finite State Machine (FSM):** Custom built AI for customers (Walking, Waiting for Menu, Eating, Leaving).
* **📡 Event-Driven Architecture (EventBus):** Decoupled systems. UI elements, order updates, and customer actions communicate via an EventBus pattern to prevent spaghetti code.
* **♻️ Object Pooling:** Zero-allocation approach for frequently instantiated objects like Coins, Plates, and Customers.
* **✨ Procedural Animations & Game Feel:** Integrated **PrimeTween** for high-performance, zero-allocation procedural animations (Squash & stretch UI, dynamic camera shakes based on plate capacity, smooth coin scatter effects).
* **📦 Modularity:** Clean folder structure and separation of concerns between visual controllers (`TableVisualController`) and logical controllers (`TableController`).

## 🎮 Core Mechanics Showcase
1.  **Dynamic Bulaşık Sistemi (Dishwashing System):** Plates stack up based on usage. The sink visually reacts (shakes dynamically) as it reaches capacity, forcing the player to manage their time.
2.  **Order Matching:** Serving the correct procedural order triggers positive visual feedback.
3.  **Economy:** Collecting scattered coins using optimized ease-curves.

## 📂 Other Projects
If you'd like to see my skills in 3D Environments and Technical Art (Shaders), please check out my 3D Horror Game project:
🔗 [Link to your 3D Horror Game Repo here]

## 📬 Contact
[Your Name/LinkedIn Link]
