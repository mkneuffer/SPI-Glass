EXTERNAL SwitchToThiefScene2()

Your job is really boring. #speaker:Archibald #portrait:archibald #layout:left
-> choice1

=== choice1 ===
+ [You aren’t obligated to come with me.]
	You aren't obligated to come with me. #speaker:Player #portrait:0
	-> continue
+ [Nothing is stopping you from leaving…] 
	Nothing is stopping you from leaving… #speaker:Player #portrait:0
	-> continue

=== continue ===
Well, I have nothing else to do… #speaker:Archibald #portrait:archibald
What were you doing before you met me then? #speaker:Player #portrait:0
Nothing. #speaker:Archibald #portrait:archibald
Okay, great. You have no life. #speaker:Player #portrait:0
? Well, yes… #speaker:Archibald #portrait:archibald
Psst. #speaker:??? #portrait:Thief
What? #speaker:Archibald #portrait:archibald
Was that not you? #speaker:Player #portrait:0
No, I thought that was you. Wait. Do you see that in the corner? #speaker:Archibald #portrait:archibald
Uh. No — oh, yes. I do. #speaker:Player #portrait:0
Time to clock in, rookie! #speaker:Archibald #portrait:archibald 
~SwitchToThiefScene2()
-> END