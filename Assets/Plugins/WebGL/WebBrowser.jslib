mergeInto(LibraryManager.library, {
  IsMobileBrowser: function () {
    return (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent));
  },
  SendShot: function (str) {
    //console.log('pluging send shot');
    SendShot(Pointer_stringify(str));
  },
  SendShotIn: function (str) {
    //console.log('pluging shot in');
    ShotIn(Pointer_stringify(str));
  },
  SendReady: function () {
    //console.log('pluging ready');
    SetPlayerReady();
  },
  SetActiveOther: function () {
    //console.log('pluging ready');
    SetActiveOther();
  },
  GameLose: function () {
    //console.log('pluging ready');
    GameLose();
  },
});
