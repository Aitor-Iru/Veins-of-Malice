# Análisis Técnico de Estilo Visual: "Clash Royale Look"

**Objetivo:** Lograr una estética "3D en entorno 2D" (estilo Clash Royale) para un juego de Lucha/RPG Lateral.

Existen tres enfoques principales para lograr este acabado visual en Unity:

| Enfoque | Descripción | Pros (Ventajas) | Contras (Desventajas) | Recomendación |
| :--- | :--- | :--- | :--- | :--- |
| **1. Sprites Pre-Renderizados (Donkey Kong Country / Dead Cells)** | Modelar y animar en 3D (Blender/Maya), capturar fotogramas y convertirlos en Sprites 2D tradicionales. | - Rendimiento excelente (solo dibuja 2D).<br>- Estilo visual exacto y controlado.<br>- Fácil de integrar con físicas 2D. | - Iteración lenta (cambiar una animación requiere re-renderizar todo).<br>- Iluminación estática (no reacciona dinámicamente a luces del juego). | **Alta** si tienes animadores 3D pero quieres lógica 2D simple. |
| **2. Modelos 3D en Tiempo Real (2.5D - Street Fighter V / Smash Bros)** | Usar modelos 3D reales, restringidos a moverse solo en ejes X e Y. Cámara Ortográfica. | - Iluminación dinámica real (sombras, brillos).<br>- Transiciones de animación fluidas (blending).<br>- Física de ropa/pelo en tiempo real. | - Requiere rigging y animación 3D completa.<br>- Mayor costo de rendimiento que sprites simples.<br>- Hitboxes más complejas de ajustar al render. | **Muy Alta** para el look "Clash Royale" (brillo, volumen). |
| **3. Sprites 2D con Mapas de Normales (Spine / 2D Lights)** | Dibujar personajes en 2D pero añadir texturas de "profundidad" (Normal Maps) para que reaccionen a la luz. | - Flujo de trabajo 2D puro.<br>- Reacciona a la luz (Rim light, sombras 2D). | - Nunca se verá "tan 3D" como un modelo real.<br>- Requiere dibujar mapas de normales a mano o generarlos por software. | Media. Buen look, pero diferente a Clash Royale. |

## Propuesta de Implementación: Enfoque Híbrido (2.5D Real-time)

Para acercarse más a Clash Royale, sugiero el **Enfoque 2 (2.5D Real-time)** o, si el equipo es pequeño, el **Enfoque 1 (Pre-Renderizado con Normal Maps)**.

### Pipeline Técnico Sugerido (2.5D):

1.  **Escena:** Usar entorno 3D pero con vista lateral bloqueada.
    *   **Cámara:** Perspectiva con FOV bajo (para aplanar) u Ortográfica.
    *   **Personajes:** Modelos 3D "Low Poly" con texturas pintadas a mano o shaders "Toon/Lit".
    *   **Shader:** Un shader personalizado (URP Lit o Toon Shader) que soporte "Rim Light" (luz de contorno) y sombras suaves.
    *   **Fondo:** Puede ser 2D (imágenes planas en capas) o 3D simple.

### Pipeline Alternativo (Sprites Prerenderizados con Normal Maps):

1.  **Modelado:** Crear personaje High Poly en 3D.
2.  **Captura:** Usar una herramienta (ej. *Sprite render tool* en Unity o Blender) para exportar:
    *   Color (Albedo)
    *   Normal Map (Dirección de luz)
    *   Emisión (Brillos mágicos)
3.  **Motor 2D:** Usar URP 2D Renderer. Aplicar Normal Maps a los sprites.
    *   *Resultado:* El personaje parece 3D y la luz le afecta en tiempo real, pero es un sprite plano.
    *   *Ejemplo:* Dead Cells usa este método (sin normal maps, pero mismo flujo).

### Decisión Requerida:
*   ¿Cuentas con modeladores/animadores 3D en el equipo? -> **Ve por 2.5D (Opción 2).**
*   ¿Solo tienes artistas 2D? -> **Opción 3 (Spine + Normals).**
*   ¿Quieres el look exacto pero simplificar código? -> **Opción 1 (Pre-render).**

Para "Veins of Malice", dado que es un RPG de Lucha, **la Opción 2 (2.5D)** permitiría las animaciones más fluidas y el impacto visual más "premium" similar a Clash Royale.
