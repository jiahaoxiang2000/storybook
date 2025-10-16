#import "@preview/touying:0.6.1": *
#import themes.simple: *
#import "@preview/cetz:0.3.2"
#import "@preview/fletcher:0.5.5" as fletcher: edge, node
#import "@preview/numbly:0.1.0": numbly
#import "@preview/theorion:0.3.2": *
#import cosmos.clouds: *
#show: show-theorion

// cetz and fletcher bindings for touying
#let cetz-canvas = touying-reducer.with(reduce: cetz.canvas, cover: cetz.draw.hide.with(bounds: true))
#let fletcher-diagram = touying-reducer.with(reduce: fletcher.diagram, cover: fletcher.hide)

// Set Chinese fonts for the presentation
// If the fonts are not installed, you can find new fonts to replace them. by the `typst fonts`.
// #set text(
//   font: (
//     "Source Han Serif SC", // Alternative Chinese serif font
//   ),
//   // lang: "zh",
//   // region: "cn",
// )

// Color shorthand functions
#let redt(content) = text(fill: red, content)
#let bluet(content) = text(fill: blue, content)
#let greent(content) = text(fill: green, content)
#let yellowt(content) = text(fill: yellow, content)
#let oranget(content) = text(fill: orange, content)
#let purplet(content) = text(fill: purple, content)
#let greyt(content) = text(fill: gray, content)
#let grayt(content) = text(fill: gray, content)

// Additional font customization options:
// For headings, you can use a different font:
// #show heading: set text(font: "Source Han Serif SC", weight: "bold")
// #show raw: set text(font: "Source Han Mono SC")

#show: simple-theme.with(aspect-ratio: "16-9", footer: [Emotion Maze])

#title-slide[
  = The Little Monster's Emotion Maze
  #v(2em)

  UX Design Reflection & Process Evaluation

  #v(1em)

  ID: 24516743

  #v(2em)
  #datetime.today().display()
]


== Project Introduction

*"The Little Monster's Emotion Maze"*

An interactive storybook app guiding children's emotional management

*Target Audience:* Ages 5–8

*Core Concept:*
- Transforms abstract emotions into observable, actionable experiences
- Based on narrative therapy and mindfulness principles
- Four emotion mazes: Anger, Sadness, Fear, Joy

// Targets ages 5-8 when children transition from feeling to naming emotions. Combines narrative therapy externalization with mindfulness. Children become "emotion helpers" navigating mazes for emotion identification and regulation practice.


== UX Design Principles

*Color Psychology as Narrative Tool*
- Each emotion mapped to distinct, carefully calibrated color palette
- Anger → Volcanic Red | Sadness → Soft Gray | Fear → Night Blue | Joy → Honey Yellow

*Child-Centered Design*
- Soft, rounded visual language with low-contrast shadows
- Touch targets ≥40px × 40px for motor control accuracy
- High-contrast text (white on emotion-specific backgrounds)

// Three core principles: (1) Color as narrative device—red=anger, blue=calm. (2) Touch targets ≥40px for developing motor skills, 10px edge spacing. (3) Layered feedback—animation, sound, character response—creates complete loop showing children their actions matter.


== Home Scene: Central Hub Design

#slide(composer: (1fr, 1fr))[
  #align(center)[
    #image("liFig/image_12.png", width: 100%)
  ]
][
  *Key UX Features:*
  - Moro as emotional proxy at center
  - Four rectangular emotion portals with high-contrast labels
  - Low-saturation warm green background (non-competing)
  - Autonomous emotion selection empowers children
]

// Home screen: "low cognitive load" principle. Children observe Moro, infer emotion, choose portal. Rectangular buttons improve legibility and reduce misclicks. Warm green background creates storybook feel while keeping emotion buttons focal. Design fosters autonomy and builds emotional inference skills.


== Anger Maze: Deep Breathing Practice

#slide(composer: (1fr, 1fr))[
  #align(center)[
    #image("liFig/image_11.png", width: 100%)
  ]
][
  *Regulation Mechanic:* "Take Deep Breaths" button (5 taps)

  *UX Highlights:*
  - Unified red color filter creates heated atmosphere
  - Real-time progress counter: "0/5 breaths"
  - Flames dim progressively with each breath
]

// Behavioral practice via concrete metaphor: anger=flame, breathing=cooling water. Each tap triggers layered feedback: animation, counter, flame dimming, Moro softening. Background music (v1.2) adds emotional resonance. Visual-auditory sync reinforces: "My actions help Moro and myself feel calmer."


== Sadness Maze: Memory & Cognitive Reframing

#slide(composer: (1fr, 1fr))[
  #align(center)[
    #image("liFig/image_8.png")
  ]
][
  *Regulation Mechanic:* Reveal 4 happy memory cards

  *UX Highlights:*
  - Gray-cyan palette evokes quiet heaviness with spark of hope
  - White "?" cards provide clear interactive cues
  - 3D flip animation reveals colorful memories
]

// Teaches sadness is natural; positive memories bring comfort. "Finding the Colors" metaphor: sadness=colorless world. 3D flip animation reveals vivid memories, environment gains warmth, Moro uncurls. White font (v1.3) improves contrast on blue-gray background for developing readers.


== Fear Maze: Gradual Light Revelation

#slide(composer: (1fr, 1fr))[
  #align(center)[
    #image("liFig/image_10.png")
  ]
][
  *Regulation Mechanic:* "Turn On Light" button (4 lamps)

  *UX Highlights:*
  - Deep, dark backdrop mirrors feeling of fear
  - Each lamp activation brightens scene by 15–20%
  - Moro's trembling decreases as light spreads
]

// Cognitive reframing through action. "Lighting Up the Dark" teaches darkness is explorable, not threatening. Each lamp lights up, shifting fear to reassurance. Deep blue button avoids anxiety-heightening saturation. Shadows reveal harmless objects—fear distorts reality. Audio creates immersion without startling; calibrated for engagement, not overwhelm.


== Joy Maze: Emotional Balance Practice

#slide(composer: (1fr, 1fr))[
  #align(center)[
    #image("liFig/image_9.png")
  ]
][
  *Regulation Mechanic:* "Calm Down" button (6 taps)

  *UX Highlights:*
  - Warm golden-orange backdrop radiates cheerful energy
  - Scene dynamics slow progressively (bouncing settles, sparkles fade)
]

// Teaches positive emotions can overwhelm; slowing down is kind and helpful. "Calming the Excitement" reframes overstimulation as natural and balanceable. Flexible interaction: tap button or floating calm points. Rhythmic tapping mirrors mindful pausing. Moro's glow softens, bouncing stops, smile becomes serene. Builds emotional intelligence: joy is wonderful, even better when calm.


== Credits Scene & Project Reflection

#slide(composer: (1fr, 1fr))[
  #align(center)[
    #image("liFig/image_7.png")
  ]][
  *Achievement Celebration:* "Emotion Hero!"

  *UX Highlights:*
  - Warm peach-beige background with floating golden confetti
  - Moro radiates pure joy (earned through completion)
  - Positive reinforcement with emotional closure
]

// Delivers positive reinforcement and closure with warmth and pride. After intense scenarios, children end feeling capable and happy. Links celebration to problem-solving: emotions can be understood and managed. Three major refinements: rectangular buttons (operability), background music (engagement), optimized fonts (clarity)—all driven by children's developmental needs.
