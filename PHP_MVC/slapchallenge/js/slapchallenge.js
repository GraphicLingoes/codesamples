
// Document Ready
$(document).ready(function(e){
	$("#applicantsTable tr a").click(function(e){
		e.preventDefault();
		$("#fightArena").html("<h1>Fight Arena</h1>");
		// Get current applicant stats
		Health = parseInt($(this).closest("tr").children(".Health").text());
		Damage = parseInt($(this).closest("tr").children(".Damage").text());
		FullName = $(this).closest("tr").children(".FullName").text();
		Attacks = parseInt($(this).closest("tr").children(".Attacks").text());
		ID = $(this).attr("id");
			
		// load fight array with other applicants and their stats
		var challengers = [];
		$("#applicantsTable tbody tr").each(function(i, rows){
			var $row = $(rows);
			var foeFullName = $row.find(".FullName").text();
			if(foeFullName != FullName) {
				var foeID = $row.find(".FightAction").children("a").attr("id");
				var foeHealth = $row.find(".Health").text();
				var foeDamage = $row.find(".Damage").text();
				var foeAttacks = $row.find(".Attacks").text();
				var fighter = {
						"ID":foeID,
						"FullName": foeFullName,
						"Damage": foeDamage,
						"Health": foeHealth,
						"Attacks": foeAttacks
				}
				// Store opponents
				challengers.push(fighter);
			}
		});
		
		
		// Set up some flags used during challenge
		currentSlapper = "";
		applicantSlapsFirst = rollDice();
		// Iterate through slappers and fight.
		for(var i = 0; i < challengers.length; i++){
			currentRound = 1;
			$("#fightArena").append("Round " + currentRound + "<br />");
			totalAttacks = parseInt(challengers[i]["Attacks"]) + Attacks;
			tallyFoeHealth = parseInt(challengers[i]["Health"]);
			tallyCurrentApplicantHealth = Health;
			_foeID = parseInt(challengers[i]["ID"]);
			_foeHealth = parseInt(challengers[i]["Health"]);
			_foeDamage =  parseInt(challengers[i]["Damage"]);
			_foeAttacks = parseInt(challengers[i]["Attacks"]);
			_foeFullName = challengers[i]["FullName"];
			// Set current slapper
			currentSlapper = applicantSlapsFirst ? "applicant" : "foe";
			startFight();
			
			
			// Determine if more rounds are needed
			
			
			$("#fightArena").append("<br />End of fight<br />");
		}
	})
	
	function startFight() {
		//inner loop for slap match
		for(j = 0; j <= totalAttacks; j++) {
			if (currentSlapper == "applicant") {
				if (j == 0 || j/2 < Attacks ) {
					var html = FullName + " slaps " + _foeFullName;
					html += " for " + Damage + " damage. ";
					html += "(" + tallyFoeHealth + " => ";
					tallyFoeHealth = tallyFoeHealth - Damage;
					html += tallyFoeHealth;
					html += ")<br />";
					
					$("#fightArena").append(html);
					// Check to see if foeHealth has reached zero yet
					if(tallyFoeHealth <= 0) {
						determineWinner();
						return;
					}
				
				}
				//Change slapper
				currentSlapper = "foe";
			} else {
				if (j == 0 || j/2 < _foeAttacks) {
					var html = _foeFullName + " slaps " + FullName;
					html += " for " + _foeDamage + " damage. ";
					html += "(" + tallyCurrentApplicantHealth + " => ";
					tallyCurrentApplicantHealth = tallyCurrentApplicantHealth - _foeDamage;
					html += tallyCurrentApplicantHealth;
					html += ")<br />";
					
					$("#fightArena").append(html);
					// Check to see if tallyCurrentApplicantHealth has reached zero yet
					if(tallyCurrentApplicantHealth <= 0) {
						determineWinner();
						return;
					}
				}
				// Change slapper
				currentSlapper = "applicant";
			}
		}
		// Prepare Round Two
		currentRound++;
		// Check to see if fight is over.
		if (!determineWinner()) {
			startFight();
		}
	}
	
	function determineWinner() {
		if (tallyCurrentApplicantHealth > 0 && tallyFoeHealth > 0) {
			$("#fightArena").append("Round " + currentRound + "<br />");
			return false;
		} else if (tallyCurrentApplicantHealth > 0 && tallyFoeHealth <= 0) {
			$("#fightArena").append("<br /><strong>" + FullName + " Wins!</strong>");
			postWinner({
				"idApplicant": ID,
				"idFoe": _foeID,
				"Win": 1,
				"Loss": 0
			});
			return true;
		} else if (tallyCurrentApplicantHealth <= 0 && tallyFoeHealth > 0) {
			$("#fightArena").append("<br /><strong>" + _foeFullName + " Wins!</strong>");
			postWinner({
				"idApplicant": parseInt(ID),
				"idFoe": _foeID,
				"Win": 0,
				"Loss": 1
			});
			return true;
		} else {
			console.log("something went wrong");
			return true;
		}
	}
	
	function rollDice() {
		return Math.floor((Math.random() * 100) + 1) > Math.floor((Math.random() * 100) + 1);
	}
	
	function postWinner(params){
		 $.ajax({
	          url: 'http://slapchallenge.dev:82/api/fight/addChallenges',
	          type: 'POST',
	          accepts: "text/html",
	          contentType: "application/json",
	          data: JSON.stringify({
	            "idApplicant": params["idApplicant"],
	            "idFoe": params["idFoe"],
	            "Win": params["Win"],
	            "Loss": params["Loss"]
	          }),
	          success: function(data) {
	              console.log("Success");
	          },
	          error: function(xhr, errorStatus, error) {
	              alert(errorStatus + " " + error);
	          }
	      });
	}
});