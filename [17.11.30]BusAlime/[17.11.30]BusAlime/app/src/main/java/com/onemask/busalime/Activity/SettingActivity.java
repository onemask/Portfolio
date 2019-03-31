package com.onemask.busalime.Activity;

import android.content.Intent;
import android.graphics.drawable.ColorDrawable;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.SeekBar;
import android.widget.TextView;
import android.widget.Toast;

import com.soo.busalime.R;

/**
 * Created by kmj on 2017-09-22.
 */

public class SettingActivity extends AppCompatActivity {

    TextView    txt,txt2,dist,txt3,txt4,txt5,txt6;
    private SeekBar sb;
    private int distance;


    Button btn;
    public static final int REQUEST_CODE_RINGTONE = 10005;
    private Uri uri=null;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.settings);

        final Intent intent = getIntent();
        distance =intent.getIntExtra("distance", 100);

        //액션바 타이틀 변경하기
        getSupportActionBar().setTitle("환경설정");
        //홈버튼 표시
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        getSupportActionBar().setBackgroundDrawable(new ColorDrawable(0xFF339999));

        txt = (TextView) findViewById(R.id.txt);
        txt2 = (TextView) findViewById(R.id.txt2);
        txt3 = (TextView) findViewById(R.id.max);
        txt4 = (TextView) findViewById(R.id.min);
        txt5 = (TextView) findViewById(R.id.music);
        txt6 = (TextView) findViewById(R.id.distCont);
        dist = (TextView) findViewById(R.id.dist);

        btn = (Button) findViewById(R.id.lib);

        sb = (SeekBar) findViewById(R.id.seekBar);
        sb.setProgress(distance);
        dist.setText(String.valueOf(distance));


        sb.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {

            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                //TextView에 출력하는 메서드
                printSelected(progress + 100);
            }

            public void onStartTrackingTouch(SeekBar seekBar) {
                //선택막대를 터치하고 드래그를 시작할때 실행되는 메서드
            }

            public void onStopTrackingTouch(SeekBar seekBar) {
                doAfterTrack(); //선택이 완료했음을 표시하는 메서드
            }
        });

        btn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View arg0) {
                //버스 폰에있는 알람 가져오기
                Intent intent = new Intent(RingtoneManager.ACTION_RINGTONE_PICKER);
                intent.putExtra(RingtoneManager.EXTRA_RINGTONE_TITLE, "알람 벨소리를  선택하세요");
                intent.putExtra(RingtoneManager.EXTRA_RINGTONE_TYPE, RingtoneManager.TYPE_ALL);
                intent.putExtra(RingtoneManager.EXTRA_RINGTONE_SHOW_SILENT, false);
                intent.putExtra(RingtoneManager.EXTRA_RINGTONE_DEFAULT_URI,false);
                intent.putExtra(RingtoneManager.EXTRA_RINGTONE_SHOW_DEFAULT, false);
                startActivityForResult(intent, REQUEST_CODE_RINGTONE);
            }


        });

    }//oncreate 끝

    //액션버튼 메뉴 액션바에 집어 넣기
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.save, menu);
        return true;
    }

    //   //벨소리가져오기키
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent intent) {
        super.onActivityResult(requestCode, resultCode, intent);
        switch(requestCode) {
            case REQUEST_CODE_RINGTONE :
                if(resultCode == RESULT_OK ) {
                    uri = intent.getParcelableExtra(RingtoneManager.EXTRA_RINGTONE_PICKED_URI);
                }
        }
    }

    //seekbar 조절할때 필요한 메소드

    public void printSelected(int value){
        TextView tv=(TextView) findViewById(R.id.dist);
        tv.setText(String.valueOf(value));
        setDistance(value);
    }

    private void setDistance(int value){
        if (value<=0){
            value=0;}
        else if(value>=899){
            value=899;}
        distance=value;    }

    private void doAfterTrack(){
        dist.setText(dist.getText());
    }


    //저장 눌렀을때
    @Override
    public boolean onOptionsItemSelected(MenuItem item){
        int id = item.getItemId();
        Intent intent = new Intent();
        //or switch문을 이용하면 될듯 하다.
        switch (id){
            case R.id.save:
                Toast.makeText(this, "저장되었습니다", Toast.LENGTH_LONG).show();
                setDistance(distance);
                //여기서 인텐트 넘겨주는거 지정해줌
                // 결과값을 intent에 담아서
                intent.putExtra("result",distance);
                intent.setData(uri);
                // setResult에 넘겨준다.
                setResult(RESULT_OK, intent);

                // 현재 activity를 종료한다.
                finish();  //finish()를 하면 onActivityResult 호출
            default:
                setResult(RESULT_CANCELED, intent);

        }
        if (id==android.R.id.home)
            finish();
        return super.onOptionsItemSelected(item);
    }

}
