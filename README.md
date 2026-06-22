<div align="center">
🧠 Unity-PlayToMemorizeGame

A card-matching memory game where the content is generated, not hardcoded.

Names come from a local LLM. Images come from a local Stable Diffusion instance.
Built and run through the Unity Editor.

</div>

<div align="center">
  <em>📸 Screenshot / gameplay GIF placeholder — drop one in <code>docs/</code> and link it here.</em>
</div>

What this is

Two connected, AI-assisted loops live in this project:

Name QuizCard MatchingInputA name you type + a reference image you importA text prompt you typeAI usedOllama (openchat model)Stable Diffusion (AUTOMATIC1111 WebUI)What the AI produces3 decoy names similar to your inputA generated imageSaved touserData.jsonAssets/imageGenAi/Played asMultiple-choice quiz — pick the real name out of 4 optionsClassic memory-match — flip pairs of cards on a timer

The two loops are independent: the matching game just reads whatever's sitting in Assets/imageGenAi/, whether those images came from a quiz session or were generated on their own.

<details>
<summary><b>🔄 See the full data flow</b></summary>
#mermaid-r13r-r1 { font-family: "Anthropic Sans", system-ui, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; font-size: 16px; fill: rgb(229, 229, 229); }
#mermaid-r13r-r1 .edge-animation-slow { stroke-dashoffset: 900; animation: 50s linear 0s infinite normal none running dash; stroke-linecap: round; stroke-dasharray: 9, 5 !important; }
#mermaid-r13r-r1 .edge-animation-fast { stroke-dashoffset: 900; animation: 20s linear 0s infinite normal none running dash; stroke-linecap: round; stroke-dasharray: 9, 5 !important; }
#mermaid-r13r-r1 .error-icon { fill: rgb(204, 120, 92); }
#mermaid-r13r-r1 .error-text { fill: rgb(51, 135, 163); stroke: rgb(51, 135, 163); }
#mermaid-r13r-r1 .edge-thickness-normal { stroke-width: 1px; }
#mermaid-r13r-r1 .edge-thickness-thick { stroke-width: 3.5px; }
#mermaid-r13r-r1 .edge-pattern-solid { stroke-dasharray: 0; }
#mermaid-r13r-r1 .edge-thickness-invisible { stroke-width: 0; fill: none; }
#mermaid-r13r-r1 .edge-pattern-dashed { stroke-dasharray: 3; }
#mermaid-r13r-r1 .edge-pattern-dotted { stroke-dasharray: 2; }
#mermaid-r13r-r1 .marker { fill: rgb(161, 161, 161); stroke: rgb(161, 161, 161); }
#mermaid-r13r-r1 .marker.cross { stroke: rgb(161, 161, 161); }
#mermaid-r13r-r1 svg { font-family: "Anthropic Sans", system-ui, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; font-size: 16px; }
#mermaid-r13r-r1 p { margin: 0px; }
#mermaid-r13r-r1 .label { font-family: "Anthropic Sans", system-ui, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; color: rgb(229, 229, 229); }
#mermaid-r13r-r1 .cluster-label text { fill: rgb(51, 135, 163); }
#mermaid-r13r-r1 .cluster-label span { color: rgb(51, 135, 163); }
#mermaid-r13r-r1 .cluster-label span p { background-color: transparent; }
#mermaid-r13r-r1 .label text, #mermaid-r13r-r1 span { fill: rgb(229, 229, 229); color: rgb(229, 229, 229); }
#mermaid-r13r-r1 .node rect, #mermaid-r13r-r1 .node circle, #mermaid-r13r-r1 .node ellipse, #mermaid-r13r-r1 .node polygon, #mermaid-r13r-r1 .node path { fill: transparent; stroke: rgb(161, 161, 161); stroke-width: 1px; }
#mermaid-r13r-r1 .rough-node .label text, #mermaid-r13r-r1 .node .label text, #mermaid-r13r-r1 .image-shape .label, #mermaid-r13r-r1 .icon-shape .label { text-anchor: middle; }
#mermaid-r13r-r1 .node .katex path { fill: rgb(0, 0, 0); stroke: rgb(0, 0, 0); stroke-width: 1px; }
#mermaid-r13r-r1 .rough-node .label, #mermaid-r13r-r1 .node .label, #mermaid-r13r-r1 .image-shape .label, #mermaid-r13r-r1 .icon-shape .label { text-align: center; }
#mermaid-r13r-r1 .node.clickable { cursor: pointer; }
#mermaid-r13r-r1 .root .anchor path { stroke-width: 0; stroke: rgb(161, 161, 161); fill: rgb(161, 161, 161) !important; }
#mermaid-r13r-r1 .arrowheadPath { fill: rgb(11, 11, 11); }
#mermaid-r13r-r1 .edgePath .path { stroke: rgb(161, 161, 161); stroke-width: 1px; }
#mermaid-r13r-r1 .flowchart-link { stroke: rgb(161, 161, 161); fill: none; }
#mermaid-r13r-r1 .edgeLabel { background-color: transparent; text-align: center; }
#mermaid-r13r-r1 .edgeLabel p { background-color: transparent; }
#mermaid-r13r-r1 .edgeLabel rect { opacity: 0.5; background-color: transparent; fill: transparent; }
#mermaid-r13r-r1 .labelBkg { background-color: rgba(0, 0, 0, 0.5); }
#mermaid-r13r-r1 .cluster rect { fill: rgb(204, 120, 92); stroke: rgb(138, 115, 107); stroke-width: 1px; }
#mermaid-r13r-r1 .cluster text { fill: rgb(51, 135, 163); }
#mermaid-r13r-r1 .cluster span { color: rgb(51, 135, 163); }
#mermaid-r13r-r1 div.mermaidTooltip { position: absolute; text-align: center; max-width: 200px; padding: 2px; font-family: "Anthropic Sans", system-ui, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; font-size: 12px; background: rgb(204, 120, 92); border: 1px solid rgb(138, 115, 107); border-radius: 2px; pointer-events: none; z-index: 100; }
#mermaid-r13r-r1 .flowchartTitleText { text-anchor: middle; font-size: 18px; fill: rgb(229, 229, 229); }
#mermaid-r13r-r1 rect.text { fill: none; stroke-width: 0; }
#mermaid-r13r-r1 .icon-shape, #mermaid-r13r-r1 .image-shape { background-color: transparent; text-align: center; }
#mermaid-r13r-r1 .icon-shape p, #mermaid-r13r-r1 .image-shape p { background-color: transparent; padding: 2px; }
#mermaid-r13r-r1 .icon-shape .label rect, #mermaid-r13r-r1 .image-shape .label rect { opacity: 0.5; background-color: transparent; fill: transparent; }
#mermaid-r13r-r1 .label-icon { display: inline-block; height: 1em; overflow: visible; vertical-align: -0.125em; }
#mermaid-r13r-r1 .node .label-icon path { fill: currentcolor; stroke: revert; stroke-width: revert; }
#mermaid-r13r-r1 .node .neo-node { stroke: rgb(161, 161, 161); }
#mermaid-r13r-r1 [data-look="neo"].node rect, #mermaid-r13r-r1 [data-look="neo"].cluster rect, #mermaid-r13r-r1 [data-look="neo"].node polygon { stroke: url("#mermaid-r13r-r1-gradient"); filter: drop-shadow(rgb(185, 185, 185) 1px 2px 2px); }
#mermaid-r13r-r1 [data-look="neo"].node path { stroke: url("#mermaid-r13r-r1-gradient"); stroke-width: 1px; }
#mermaid-r13r-r1 [data-look="neo"].node .outer-path { filter: drop-shadow(rgb(185, 185, 185) 1px 2px 2px); }
#mermaid-r13r-r1 [data-look="neo"].node .neo-line path { stroke: rgb(161, 161, 161); filter: none; }
#mermaid-r13r-r1 [data-look="neo"].node circle { stroke: url("#mermaid-r13r-r1-gradient"); filter: drop-shadow(rgb(185, 185, 185) 1px 2px 2px); }
#mermaid-r13r-r1 [data-look="neo"].node circle .state-start { fill: rgb(0, 0, 0); }
#mermaid-r13r-r1 [data-look="neo"].icon-shape .icon { fill: url("#mermaid-r13r-r1-gradient"); filter: drop-shadow(rgb(185, 185, 185) 1px 2px 2px); }
#mermaid-r13r-r1 [data-look="neo"].icon-shape .icon-neo path { stroke: url("#mermaid-r13r-r1-gradient"); filter: drop-shadow(rgb(185, 185, 185) 1px 2px 2px); }
#mermaid-r13r-r1 :root { --mermaid-font-family: "Anthropic Sans",system-ui,"Segoe UI",Roboto,Helvetica,Arial,sans-serif; }Card Matchingshared folderName QuizType a name + importimageOllama: openchat3 decoy names generatedSaved to userData.jsonQuizGame: pick the realnameType an image promptStable Diffusion APIImage saved toAssets/imageGenAi/SceneController: build 2x4gridFisher-Yates shuffleFlip pairs against a 20stimer

</details>

Requirements

You need both of these running locally alongside the Unity Editor:

DependencyPowersDefault endpoint🦙 Ollama with openchat pulledName Quizhttp://localhost:11434🎨 AUTOMATIC1111 SD WebUI (--api flag)Card Matchinghttp://127.0.0.1:7860


If either service isn't running, that half of the project fails quietly (console errors only) rather than crashing — see Known Limitations.




Getting Started

bash# 1. Pull the model the quiz needs
ollama pull openchat

# 2. Launch SD WebUI with the API enabled
./webui.sh --api          # webui-user.bat --api on Windows

Then:


Open the project in Unity (📌 add your Unity version — check ProjectSettings/ProjectVersion.txt).
Press Play.
Generate a few names/images via the Name Quiz and/or image prompt screen — the matching game needs at least 4 unique images in Assets/imageGenAi/ before it's playable.



⚠️ Known Limitations

<details open>
<summary>Click to collapse</summary>

Editor-only. OnImportImageButtonClicked calls UnityEditor.EditorUtility.OpenFilePanel, an Editor-only API with no build guard around the call site. This will likely block a standalone build — image import currently only works inside the Unity Editor.
No bundled sample data. A fresh clone has no userData.json and no images in imageGenAi/. Both AI services need to run at least once to populate either game.
Ollama call is blocking. OllamaChatClient.Complete waits on .Result, so the UI briefly freezes during generation.
No exposed SD parameters. ImageGenerationManager sends only the prompt text; sampler, steps, and negative prompt all use API defaults.
No confirmed shared menu connecting the two modes in the code reviewed so far — flag if there's a unifying scene not covered here.


</details>

Tech Stack

Unity · C# · TextMeshPro · Ollama · Stable Diffusion (AUTOMATIC1111) · Newtonsoft.Json
