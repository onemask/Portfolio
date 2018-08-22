package com.memoming.busalime.Activity;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.media.MediaPlayer;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.os.Vibrator;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.CameraPosition;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.MarkerOptions;
import com.memoming.busalime.GPS.Distance;
import com.memoming.busalime.GPS.GPSTracker;
import com.memoming.busalime.Map.ConvertToAdress;
import com.memoming.busalime.R;
import com.memoming.busalime.Station.Station;


public class StationActivity extends AppCompatActivity implements OnMapReadyCallback {


    private static final int B_ACTIVITY = 0;

    Station     currentStation;
    TextView    stNmTv, distanceTv, currLocationTv;
    Button      exitBtn, locateBtn, busBtn, alramBtn, refreshBtn;


    GPSTracker  gps;
    ConvertToAdress address;
    String add= "현주소를 변환하지 못했습니다.";
    Uri uri;
    private int criteria=100;

    Handler     mHandler;
    final int[] selected={0};

    public static final int RENEW_GPS = 1;
    public static final int SEND_PRINT = 2;

    Distance distanceCalculator;
    MediaPlayer mPlayer;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_station);
        Intent intent = this.getIntent();
        uri= RingtoneManager.getDefaultUri(RingtoneManager.TYPE_RINGTONE);
        mPlayer =MediaPlayer.create(this, uri);

        //this.getIntent().setData(RingtoneManager.getDefaultUri(RingtoneManager.TYPE_RINGTONE));
        // uri=intent.getData();


        //액션바 타이틀 변경하기
        getSupportActionBar().setTitle("안내양");
        //홈버튼 표시
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);

        SupportMapFragment mapFragment = (SupportMapFragment) getSupportFragmentManager().findFragmentById(R.id.mapView);
        mapFragment.getMapAsync(this);
        //!-------------alarm----------------------------------------------
        final Vibrator    vibrator =(Vibrator)getSystemService(Context.VIBRATOR_SERVICE);


        currentStation  = (Station)     intent.getSerializableExtra("station");
        stNmTv          = (TextView)    findViewById(R.id.stNmTextView);
        distanceTv      = (TextView)    findViewById(R.id.distanceTv);
        exitBtn         = (Button)      findViewById(R.id.exitBtn);
        alramBtn        = (Button)      findViewById(R.id.alramBtn);

        currLocationTv = (TextView)    findViewById(R.id.currLocation);
        locateBtn        = (Button)      findViewById(R.id.locateBtn);
        refreshBtn        = (Button)      findViewById(R.id.refreshBtn);
        busBtn        = (Button)      findViewById(R.id.busBtn);

        distanceCalculator = new Distance();
        address=new ConvertToAdress();

        stNmTv.setText("정류소명 : " +currentStation.getStNm());

        SupportMapFragment mapFragment2 = (SupportMapFragment) getSupportFragmentManager().findFragmentById(R.id.mapView);
        mapFragment2.getMapAsync(this);


        exitBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                finish();
            }
        });

        // < ---------------  GPS ---------------- >

        if ( Build.VERSION.SDK_INT >= 23 && //SDK 버전이 23보다 이상이면 계속 위치정보 허가 요청
            ContextCompat.checkSelfPermission( this, android.Manifest.permission.ACCESS_FINE_LOCATION ) != PackageManager.PERMISSION_GRANTED ) {
            ActivityCompat.requestPermissions( this, new String[] {  android.Manifest.permission.ACCESS_FINE_LOCATION  }, 0 );
        }
        mHandler = new Handler(){
            @Override
            public void handleMessage(Message msg){
                if(msg.what==RENEW_GPS){            //GPS 갱신
                    makeNewGpsService();
                }
                if(msg.what==SEND_PRINT){
                    displayGPS();                   //현재 GPS 보여줌
                }
            }
        };

        alramBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View arg0) {

                if(gps == null)
                    gps = new GPSTracker(StationActivity.this, mHandler);
                else
                    gps.Update();

                if( !gps.canGetLocation() )
                    gps.showSettingsAlert();
            }
        });

        //!----------------------------------Alarm ----------------------
        alramBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View arg0) {

                final String[] versionArray=new String[]{"진동","소리","진동+소리"};
                AlertDialog.Builder dlg=new AlertDialog.Builder(StationActivity.this);
                dlg.setTitle("알람설정");
                dlg.setSingleChoiceItems(versionArray,0,
                        new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialogInterface, int which) {
                                selected[0]=which;
                                if(selected[0]==0){
                                    mPlayer.pause();
                                    long millisecond=1000;
                                    vibrator.vibrate(millisecond);
                                }
                                if(selected[0]==1){
                                    mPlayer.start();
                                }
                                if(selected[0]==2){
                                    long millisecond=1000;
                                    vibrator.vibrate(millisecond);
                                    mPlayer.start();
                                }
                            }
                        });

                dlg.setPositiveButton("확인", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int which) {
                        Toast.makeText(StationActivity.this
                                ,versionArray[selected[0]]
                                ,Toast.LENGTH_SHORT).show();
                        mPlayer.pause();
                    }
                });

                dlg.setNeutralButton("취소", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int which) {
                        Toast.makeText(StationActivity.this
                                ,"취소버튼을 눌렀습니다."
                                ,Toast.LENGTH_SHORT).show();
                        mPlayer.pause();
                    }
                });

                dlg.show();

                if(gps == null)
                    gps = new GPSTracker(StationActivity.this, mHandler);
                else
                    gps.Update();
                if( !gps.canGetLocation() )
                    gps.showSettingsAlert();
            }
        }); //Alarm 닫힘

        //!-----------------------------Map----------------------------//
        //버스 위치 지도 확인
        busBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                // do something when the button is clicked
                double d1=Double.valueOf(currentStation.getGpsX()).doubleValue();
                double d2=Double.valueOf(currentStation.getGpsY()).doubleValue();

                Intent intent1 = new Intent(StationActivity.this, com.memoming.busalime.Map.MapLocation.class);
                intent1.putExtra("lat", d1);  //MapLocation 인텐트의 위도값으로 넘김
                intent1.putExtra("lon", d2);    //MapLocation 인텐트의 경도값으로 넘김
                intent1.putExtra("stop",currentStation.getStNm());
                startActivity(intent1);

            }
        });

        //현재 내 위치 지도 확인
        locateBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                double d1=gps.getLatitude();
                double d2= gps.getLongitude();
                Intent intent1 = new Intent(Intent.ACTION_VIEW, Uri.parse("geo:" + d1 + "," + d2));
                startActivity(intent1);
            }
        });


        refreshBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                // do something when the button is clicked
                makeNewGpsService();
                displayGPS ();
                displayAddress();
            }
        });

    }//oncreate 닫힘

    //액션버튼 메뉴 액션바에 집어 넣기
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu, menu);
        return super.onCreateOptionsMenu(menu);
    }

    //!--------------------------------------Function --------------------------------
    public void makeNewGpsService() {
        if(gps == null)
            gps = new GPSTracker(StationActivity.this, mHandler);
        else
            gps.Update();
        mHandler.sendEmptyMessage(StationActivity.SEND_PRINT);
    }

    public void displayAddress() {
        try {
            //도로명 주소 변환하기
            add=address.getCompleteAddressString(gps.getLatitude(), gps.getLongitude(), getApplicationContext());
            currLocationTv.setText("현재 위치 : " + add);
        }
        catch (Exception e) {
            currLocationTv.setText("현재 위치 : " + add);
            e.printStackTrace();
        }
    }

    public void displayGPS () {
        displayDistance();
    }

    public void displayDistance () {
        double stationLatitude  = Double.parseDouble(currentStation.getGpsX());
        double stationLongitude = Double.parseDouble(currentStation.getGpsY());
        double currentLatitude  = gps.getLatitude();
        double currentLongitude = gps.getLongitude();

        final Vibrator vibrator=(Vibrator)getSystemService(Context.VIBRATOR_SERVICE);



        double distance =  distanceCalculator.getDistance (
                stationLatitude,stationLongitude,
                currentLatitude,currentLongitude );

        if (distance-1000<=0){
            String convertDistance = String.format("%.2f", distance);
            distanceTv.setText("남은 거리 : " + convertDistance + " m ");
            distanceTv.setTextColor(Color.RED);
            Toast.makeText(StationActivity.this,"갱신됨",Toast.LENGTH_SHORT).show();
        }
        else{
            String convertDistance = String.format("%.2f", distance*0.001);
            distanceTv.setText("남은 거리 : " + convertDistance + " km ");
            distanceTv.setTextColor(Color.BLACK);
            Toast.makeText(StationActivity.this,"갱신됨",Toast.LENGTH_SHORT).show();
        }

        if(distance<=criteria){
            if(selected[0]==0){
                mPlayer.pause();
                long millisecond=1000;
                vibrator.vibrate(millisecond);
            }
            if(selected[0]==1){
                mPlayer.start();
            }
            if(selected[0]==2){
                long millisecond=1000;
                vibrator.vibrate(millisecond);
                mPlayer.start();
            }

            AlertDialog.Builder alarm=new AlertDialog.Builder(StationActivity.this);
            alarm.setTitle("알람");
            alarm.setMessage(currentStation.getStNm()+"정류장 도착 예정");
            alarm.setPositiveButton("확인", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialogInterface, int which) {
                    Toast.makeText(StationActivity.this,"알람이 종료되었습니다.",Toast.LENGTH_SHORT).show();
                    finish();
                    mPlayer.pause();

                    NotificationManager notificationManager = (NotificationManager) StationActivity.this.getSystemService(StationActivity.this.NOTIFICATION_SERVICE);
                    Intent intent1 = new Intent(StationActivity.this.getApplicationContext(), StationActivity.class); //인텐트 생성.

                    Notification.Builder builder = new Notification.Builder(getApplicationContext());
                    intent1.addFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP | Intent.FLAG_ACTIVITY_CLEAR_TOP);//현재 액티비티를 최상으로 올리고, 최상의 액티비티를 제외한 모든 액티비티를

                    PendingIntent pendingNotificationIntent = PendingIntent.getActivity(StationActivity.this, 0, intent1, PendingIntent.FLAG_UPDATE_CURRENT);

                    builder.setSmallIcon(R.drawable.buspush).setTicker("안내양 도착알림").setWhen(System.currentTimeMillis())
                            .setNumber(1).setContentTitle("안내양").setContentText(currentStation.getStNm()+" 정류장 도착예정입니다.");


                    notificationManager.notify(1, builder.build()); // Notification send
                }
            });
            alarm.show();
        }} //     public void displayDistance 끝


    @Override
    public void onMapReady(GoogleMap googleMap) {
        LatLng place=new LatLng(Double.valueOf(currentStation.getGpsX()).doubleValue(),Double.valueOf(currentStation.getGpsY()).doubleValue());

        googleMap.getUiSettings().setZoomControlsEnabled(true);
        googleMap.getUiSettings().setCompassEnabled(true);
        googleMap.getUiSettings().setMapToolbarEnabled(false);
        CameraPosition cameraPosition = new CameraPosition.Builder().target(place).zoom(16).build();
        googleMap.animateCamera(CameraUpdateFactory.newCameraPosition((cameraPosition)));
        // 좌표에 Marker가 꽂히게
        googleMap.addMarker(new MarkerOptions().position(place).title("도착 정류소"));

    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case B_ACTIVITY: // requestCode가 B_ACTIVITY인 케이스
                if (resultCode == RESULT_OK) { //B_ACTIVITY에서 넘겨진 resultCode가 OK일때만 실행
                    criteria= data.getIntExtra("result",criteria); //등과 같이 사용할 수 있는데, 여기서 getXXX()안에 들어있는 파라메터는 꾸러미 속 데이터의 이름표라고 보면된다.
                    try{
                        uri=data.getData();
                        mPlayer =MediaPlayer.create(this,uri);}
                    catch (Exception e){
                    }
                }
        }
        super.onActivityResult(requestCode, resultCode, data);

    }


    //환경설정 아이콘 눌렀을때.
    @Override
    public boolean onOptionsItemSelected(MenuItem item){
        int id=item.getItemId();

        switch (id) {
            case R.id.action_settings:
                Toast.makeText(this, "환경설정 선택", Toast.LENGTH_SHORT).show();
                Intent intent1 = new Intent(this, SettingActivity.class);
                intent1.putExtra("distance", criteria);
                //intent1.putExtra("path",uri.toString());
                startActivityForResult(intent1, B_ACTIVITY);
                break;
            case R.id.busbell:
                Intent launchIntent = getPackageManager().getLaunchIntentForPackage("ssj.imgebuttontest");
                startActivity(launchIntent);
        }
        if (id==android.R.id.home)
            finish();

        return super.onOptionsItemSelected(item);

    }


}
