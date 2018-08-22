package ssj.imgebuttontest;

/**
 * Created by aaa on 2017-09-12.
 */
import android.os.Bundle;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

//import com.memoming.busalime.R;

public class BellActivity extends AppCompatActivity {

    private static final String TAG = "하차벨";


    FirebaseDatabase database;
    DatabaseReference myRef;
    ImageButton imageButton1;

     Button btnFinish;
    TextView tvMessage;
   // Switch Switch;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.second_activity);


        imageButton1 = (ImageButton) findViewById(R.id.imageButton1);
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);

        ActionBar actionBar = getSupportActionBar();

        //메뉴바에 '<' 버튼이 생긴다.(두개는 항상 같이다닌다)
        actionBar.setDisplayHomeAsUpEnabled(true);
        actionBar.setHomeButtonEnabled(true);








      //  tvMessage = (TextView) findViewById(R.id.tv_message);
        btnFinish=(Button)findViewById(R.id.btnFinish);

      //  Switch = (Switch) findViewById(R.id.switch1);
        database = FirebaseDatabase.getInstance();

        myRef = database.getReference("Led");
        myRef.setValue(false);


        myRef.addValueEventListener(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                try {
                    Boolean value_red = dataSnapshot.getValue(Boolean.class);
                   // Switch.setChecked(value_red);


                } catch (Exception e) {
                    e.printStackTrace();
                }

            }

            @Override
            public void onCancelled(DatabaseError databaseError) {
                Log.w(TAG, "Failed to read value", databaseError.toException());
            }
        });
//


//
//
//



//        Switch.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
//            @Override
//            public void onCheckedChanged(CompoundButton compoundButton, boolean isChecked) {
//                if (isChecked) {
//
//                    myRef.setValue(true);
//                    imageButton1.setImageResource(R.drawable.buttton2);
//
//
//                } else {
//
//                    myRef.setValue(false);
//                    imageButton1.setImageResource(R.drawable.button1);
//
//                }
//
//            }
//        });





        imageButton1.setOnClickListener(new View.OnClickListener() {
              @Override
              public void onClick(View view) {
                  imageButton1.setImageResource(R.drawable.change2);
                  myRef.setValue(1);

                  Toast.makeText(BellActivity.this
                          ,"버스벨이 울렸습니다"
                          ,Toast.LENGTH_SHORT).show();




              }
          });















//        btnFinish.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View view) {
//                //앱종료
//;
//                finish();
//                myRef.setValue(0);
//
//            }
//        });








    }





















    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.back, menu);
        return true;
    }
    @Override
    public boolean onOptionsItemSelected(MenuItem item){
      //  Intent intent1= new Intent(this, StationActivity.class);
        int id = item.getItemId();
        //or switch문을 이용하면 될듯 하다.
         if(id==item.getItemId()){

             finish();



         }
        return super.onOptionsItemSelected(item);
    }









}

