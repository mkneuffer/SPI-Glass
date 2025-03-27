EXTERNAL SwitchToThiefMap1()

Hi, friends. #speaker:Owlish Ghost #portrait:Thief #layout:left
… Archibald, you know this guy? #speaker:Player #portrait:0
No, my friends aren’t total freaks like this guy. #speaker:Archibald #portrait:archibald
You want a job…? You look like you need some work… #speaker:Owlish Ghost #portrait:Thief
-> choice

=== choice ===
+ [I’m employed.]
	I'm employed. #speaker:Player, #portrait:0
	-> continue
+ [A job?]
	A job? #speaker:Player, #portrait:0
	-> continue
+ [Archibald, maybe he’s talking to you.] #speaker:Player, #portrait:0
	I am above working a job, human. #speaker:Archibald, #portrait:archibald
	-> continue

=== continue ===
The Nachzehrer’s Crown... heard of it…? …Want to claim it for yourself? #speaker:Owlish Ghost #portrait:Thief
No, and no— #speaker:Player #portrait:0
Hold on, the Nachzehrer’s Crown? If it’s what I’m thinking of… Yes. Human, say yes. #speaker:Archibald #portrait:archibald
Kept by the mafia… They’re bad… It’s a dangerous job… #speaker:Owlish Ghost #portrait:Thief
Well, this guy doesn’t mind it, so it won’t be a problem! #speaker:Archibald #portrait:archibald
Here… A map…  If they catch you… death is guaranteed… #speaker:Owlish Ghost #portrait:Thief
Let’s go, human! #speaker:Archibald, #portrait:archibald
Hey, I think you forgot to ask for my opinion…? #speaker:Player #portrait:0
~ SwitchToThiefMap1()

-> END