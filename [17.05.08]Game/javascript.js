
var cardArray = ['playing_card.jpg','butterfly.jpg','carrots1.jpg','crocus.jpg','fuchs.jpg','geese.jpg',
                 'goose.jpg','macro.jpg','orange_juice.jpg','plants.jpg','robin.jpg']
var cardNum = [2 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0]
var currentCardName = new Array(21)
var playMode = 2

// ----------------------------------------------------------------------------- //

function randomCardNum(){
  var flag = false;

  for(var i=1; i<=10; i++)
    if(cardNum[i] < 2){
      flag = true;
      break;
    }

  if(!flag)
    return null;

  while(flag){
    randomNum = Math.floor((Math.random()*10+1));
    if (cardNum[randomNum] <= 1){
      cardNum[randomNum]++;
      return randomNum;
      break;
    }
  }
}
// ----------------------------------------------------------------------------- //

function makeCard(){
  for(var i=1; i<=20; i++){
    if (i%4 == 1)  document.write("<br />");
    document.write("<img onclick = cardHint(" + i + ") style = \"margin-left:10px; margin-top:10px\" height = 213px width=320px id=\"img"+ String(i) + "\" src=\"pic/" + cardArray[0] + "\" />");
  }
  document.write("<br /> <br />");
  document.write("<button id=\"resetgame\"  style=\"margin-left:35%\" onclick = resetCard()> Reset game </button>");
  document.write("<button id=\"showimages\" style=\"margin-left:2%\"  onclick = showCard()> Show all images </button>");
  document.write("<button id=\"hideimages\" style=\"margin-left:2%\"  onclick = hideCard()> Hide all images </button>");
  document.write("<br /> <br />");
  resetCard();
}
// ----------------------------------------------------------------------------- //

function cardHint(i){
  if (playMode == 2){
    var selectCard = document.getElementById( 'img' + String(i) );
    selectCard.src = "pic/" + currentCardName[i];
    setTimeout(function() {hideCard()}, 1000);
  }
}
// ----------------------------------------------------------------------------- //

function resetCard(){
  for (var i=1; i<=20; i++){
    var selectCard = document.getElementById( 'img' + String(i) );
    var currentCardNum = randomCardNum();
    currentCardName[i] = cardArray[currentCardNum];

    if (playMode == 1)
      selectCard.src = "pic/" + cardArray[currentCardNum];
    else
      selectCard.src = "pic/" +  cardArray[0];
  }

  for(var i=1; i<=20; i++)
    cardNum[i] = 0;
}
// ----------------------------------------------------------------------------- //

function showCard(){
  playMode = 1;
  for (var i=1; i<=20; i++){
    var selectCard = document.getElementById( 'img' + String(i) );
    selectCard.src = "pic/" + currentCardName[i];
  }
}
// ----------------------------------------------------------------------------- //

function hideCard(){
  playMode = 2;
  for (var i=1; i<=20; i++){
    var selectCard = document.getElementById( 'img' + String(i) );
    selectCard.src = "pic/" + cardArray[0];
  }
}
// ----------------------------------------------------------------------------- //
