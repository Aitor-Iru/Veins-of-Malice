# Roadmap Técnico: Veins of Malice (Vertical Slice)

**Rol:** Lead Game Designer / Product Manager  
**Objetivo:** Guía de implementación granular desde concepto hasta Vertical Slice.

Este documento sirve como hoja de ruta viva para el desarrollo. Marcar las casillas `[x]` a medida que se completen las tareas.

## Fase 1: Pre-Producción y Configuración (Semanas 1-2)

### 1.1. Configuración del Proyecto (Unity)
- [x] **Repositorio y Control de Versiones**
    - [x] Inicializar repositorio Git con `.gitignore` adecuado para Unity.
    - [x] **Workflow Simplificado:** Trabajar directamente en rama `main` (Solo Dev).
    - [x] Definir estructura de carpetas (Scripts, Prefabs, Art, Audio, Scenes).
- [ ] **Definición de Pipeline de Arte (2.5D)**
    - [ ] Configurar URP (Universal Render Pipeline) con soporte para Lit Shaders.
    - [ ] Establecer estándares de importación para Modelos 3D (.fbx, .blend) y Materiales.
    - [ ] Configurar Culling Masks y Layers para separar Gameplay (3D) de UI (Overlay).
- [ ] **Prototipo de "Greybox"**
    - [ ] Crear escena de prueba con geometría básica (cubos/placeholders).
    - [ ] Implementar cámara 2D básica (Cinemachine confiner).

### 1.2. Arquitectura del Código
- [ ] **Game Manager**
    - [ ] Implementar patrón Singleton para `GameManager`.
    - [ ] Crear máquina de estados global (MainMenu, Gameplay, Pause, GameOver).
- [ ] **Input System**
    - [ ] Configurar Unity Input System (New).
    - [ ] Mapear acciones: Mover, Saltar, Atacar, Dash, Interactuar, Pausa.

---

## Fase 2: Core Gameplay - "Los Cimientos" (Semanas 3-6)

### 2.1. Controlador del Personaje (Kairo)
- [ ] **Movimiento Básico**
    - [ ] Implementar movimiento horizontal con física (Rigidbody2D).
    - [ ] Implementar salto (y doble salto si aplica al inicio).
    - [ ] Implementar Dash (con cooldown y consumo de energía).
- [ ] **Sistema de Combate (Base)**
    - [ ] Hitboxes y Hurtboxes: Sistema de detección de daño.
    - [ ] Ataque Básico: Combo de 3 golpes (lógica de ventana de input).
    - [ ] Bloqueo/Parry: Reducción de daño y ventana de "Parry Perfecto".
- [ ] **Animación (Mecanim 3D)**
    - [ ] Configurar Animator Controller para el Modelo 3D (Humanoid/Generic).
    - [ ] Implementar estados de ataque y blending de animaciones (Idle -> Run -> Attack).

### 2.2. Sistema de Enemigos (IA Básica)
- [ ] **Arquitectura de IA**
    - [ ] Crear clase base `Enemy` (hereda de `MonoBehaviour` o clase `Entity`).
    - [ ] Implementar Máquina de Estados Finita (FSM): Patrulla, Persecución, Ataque, Muerte.
- [ ] **Maldición Grado 4 (Masilla)**
    - [ ] Implementar comportamiento simple (caminar hacia el jugador, atacar en rango).
    - [ ] Hit detection y feedback visual (parpadeo al recibir daño).

### 2.3. UI y Feedback (HUD)
- [ ] **HUD Básico**
    - [ ] Barra de Vida (Health Bar) con animación de pérdida de vida.
    - [ ] Barra de Energía Maldita (Cursed Energy).
- [ ] **Feedback de Combate**
    - [ ] Números de daño flotantes (Floating Text).
    - [ ] Screen Shake sutil al golpear.

---

## Fase 3: Producción - Vertical Slice "Barrio Deformado" (Semanas 7-12)

### 3.1. Diseño de Nivel y Arte (Estilo Clash Royale)
- [ ] **Escenario: Barrio Deformado (3D)**
    - [ ] Importar assets 3D optimizados (Low Poly, Toon Shader).
    - [ ] Implementar iluminación en tiempo real (Directional Light, Point Lights para efectos mágicos).
    - [ ] Colocar props 3D y obstáculos destructibles con físicas.
- [ ] **Arte de Personajes (3D)**
    - [ ] Reemplazar placeholders de Kairo con modelo 3D final y texturas PBR/Toon.
    - [ ] Integrar modelos de Enemigo Grado 4 y 3 con Materiales Lit.
- [ ] **VFX (Efectos Visuales)**
    - [ ] Partículas de "Esencia Maldita" al golpear.
    - [ ] Efecto de "Dash" (rastro o fantasma).

### 3.2. Sistemas RPG y Narrativa
- [ ] **Sistema de Diálogo**
    - [ ] Crear sistema de cajas de texto (NPCs).
    - [ ] Integrar un NPC aliado (ej. Rion o Mentor) con diálogo ramificado simple.
- [ ] **Progresión (Vertical Slice Scope)**
    - [ ] Drop de "Esencia Maldita" al matar enemigos.
    - [ ] Menú simple de mejora de habilidades (Mockup funcional).
- [ ] **Puntos de Guardado**
    - [ ] Implementar "Santuarios" o puntos de control que guarden estado básico (Vida, Posición).

### 3.3. Enemigo Elite / Mini-Boss
- [ ] **Maldición Grado 2 (Mini-Boss)**
    - [ ] Diseñar patrón de ataque de 2 fases.
    - [ ] Fase 1: Ataques físicos pesados.
    - [ ] Fase 2: Ataque de área o proyectil de energía.
    - [ ] Barra de vida de jefe en UI.

---

## Fase 4: Polish y Cierre de Vertical Slice (Semanas 13-14)

### 4.1. Audio
- [ ] **SFX (Efectos de Sonido)**
    - [ ] Pasos (cambian según superficie).
    - [ ] Impactos de combate y voces de esfuerzo (Grr, Ha!).
    - [ ] Feedback de UI (Hover, Click).
- [ ] **Banda Sonora y Ambience**
    - [ ] Loop de música de exploración (Tenso/Misterioso).
    - [ ] Loop de música de combate (Dinámico si es posible, o transición por trigger).

### 4.2. Menús y Flujo de Juego
- [ ] **Menú Principal**
    - [ ] Botones: Jugar, Opciones, Salir.
    - [ ] Arte de fondo animado.
- [ ] **Game Loop Completo**
    - [ ] Inicio -> Tutorial/Intro -> Gameplay -> Boss -> Pantalla de "Gracias por jugar".
    - [ ] Pantalla de Pause funcional.
    - [ ] Pantalla de Game Over con opción de "Reintentar".

### 4.3. Optimización y QA
- [ ] **Performance**
    - [ ] Verificar framerate estable (60 FPS).
    - [ ] Optimizar texturas y audios (compresión).
- [ ] **Bug Fixing**
    - [ ] Playtesting intensivo de colisiones y combate.
    - [ ] Ajustar "Coyote Time" y "Jump Buffer" para mejorar el feel del salto.

---

## Checklist de Validación (Definition of Done) para Vertical Slice

- [ ] El jugador puede moverse, saltar y dashear fluidamente.
- [ ] El combate se siente responsivo (golpes, hits, muerte de enemigos).
- [ ] Hay un principio y un final claro en la demo.
- [ ] La UI comunica claramente el estado del jugador (Vida/Energía).
- [ ] No existen bugs bloqueantes (softlocks o crashes).
- [ ] El arte y el audio están implementados y son coherentes con la visión "Dark Stylized".
