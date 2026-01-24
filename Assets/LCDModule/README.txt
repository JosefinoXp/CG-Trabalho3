# LCD Module for Unity

Version: 1.0
Author: Rebush
Unity Version: 6

---

## Project Structure
LCD_Module
├─LCDModule.prefab             // prefab
├─ DemoScene
│   └─ LCD_Demo.unity          // Sample scene demonstrating the module
├─ Model
│   └─ LCD.fbx                 // 3D model of the LCD module
├─ Script
│   ├─ LCDController.cs        // Controls animation and display behavior
│   ├─ LCDModule.cs            // Core class handling sprite and rendering
│   └─ SensorDataProvider.cs   // Simulated temperature & humidity sensor data
└─ Texture
    └─ Bitmap5_8.png           // Character atlas (5x8 dot matrix)

---

## Overview

The LCD Module is a lightweight and customizable 5×8 dot-matrix display system for Unity.	The LCD Module is a lightweight, customizable 5×8 dot-matrix display system for Unity.
It emulates a compact electronic LCD panel with adjustable colors and animation timing.	It emulates a small electronic LCD panel with adjustable color, and animation timing.
This module is ideal for retro-style UIs, embedded device simulations, and sci-fi interfaces.

---

## Quick Start

Open the scene:
DemoScene/LCD_Demo.unity

Press Play to start the demo animation.

Adjust settings in the Inspector:

LCDController - manages display modes

SensorDataProvider - generates sample temperature and humidity data

---

## Main Scripts

### LCDController.cs

Handles overall display logic and animation sequences.	Handles animation and display logic.

Controls demo messages such as:
  "INITIALIZE..."
  "LOADING MODULES"
  "SYSTEM READY"

Supports real-time data updates via SensorDataProvider

Selectable display modes (plain text, animation, date/time, external data)

### LCDModule.cs

Responsible for sprite generation and rendering of the LCD display.	Responsible for generating and managing sprites for each display cell.
Public Methods:

Public Methods

SetSTR(string row1, string row2)
Updates text on the LCD

SetColor(Color color)
Changes color while preserving transparency

SetAlpha(float alpha)
Adjusts display transparency (0-100%)

RestoreInitialColors()
Restores the original color state

### SensorDataProvider.cs

Simulates live sensor input for demo purposes.

Periodically sends temperature and humidity values

Raises the event OnDataChanged(float temp, float hum)

Can be replaced with real sensor input for practical applications

---

## Customization

Replace Bitmap5_8.png with your own character atlas.

Adjust animation speed or sequence timing in LCDController.cs.

Connect to external data sources through SensorDataProvider.

---

### License

This asset is provided free of charge and may be used for both personal and commercial projects.
You are free to modify the included source code, models, and textures to fit your own needs.
No credit or attribution is required.

However, redistribution of this asset, in whole or in part, is strictly prohibited,
whether modified or unmodified.

### Allowed

Use in personal or commercial Unity projects

Modify or adapt the code, models, or textures

Include it within your own game or interactive content

### Not Allowed

Reselling or redistributing this asset as-is or with minor modifications

Including it in any other free or paid asset package

Sharing the original or modified files separately from your project

All rights to the original work remain with the creator.