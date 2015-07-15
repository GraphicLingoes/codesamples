    select
        sum(x3.is_male_0_9_under18) as is_male_0_9_under18,
        sum(x3.is_female_0_9_under18) as is_female_0_9_under18,
        
        sum(x3.is_male_0_9_ageG2) as is_male_0_9_ageG2,
        sum(x3.is_female_0_9_ageG2) as is_female_0_9_ageG2,

        sum(x3.is_male_0_9_ageG3) as is_male_0_9_ageG3,
        sum(x3.is_female_0_9_ageG3) as is_female_0_9_ageG3,

        sum(x3.is_male_0_9_ageG4) as is_male_0_9_ageG4,
        sum(x3.is_female_0_9_ageG4) as is_female_0_9_ageG4,
    
        sum(x3.is_male_0_9_ageG5) as is_male_0_9_ageG5,
        sum(x3.is_female_0_9_ageG5) as is_female_0_9_ageG5,

        sum(x3.is_male_0_9_ageG6) as is_male_0_9_ageG6,
        sum(x3.is_female_0_9_ageG6) as is_female_0_9_ageG6,

        sum(x3.is_male_0_9_ageG7) as is_male_0_9_ageG7,
        sum(x3.is_female_0_9_ageG7) as is_female_0_9_ageG7
    from
    (
        select
            if(x2.is_miles_0_9, if(x2.age between 0 and 17, if(x2.gender = 'male',1,0),0),0) as is_male_0_9_under18,
            if(x2.is_miles_0_9, if(x2.age between 0 and 17, if(x2.gender = 'female',1,0),0),0) as is_female_0_9_under18,

            if(x2.is_miles_0_9, if(x2.age between 18 and 24, if(x2.gender = 'male',1,0),0),0) as is_male_0_9_ageG2,
            if(x2.is_miles_0_9, if(x2.age between 18 and 24, if(x2.gender = 'female',1,0),0),0) as is_female_0_9_ageG2,

            if(x2.is_miles_0_9, if(x2.age between 25 and 34, if(x2.gender = 'male',1,0),0),0) as is_male_0_9_ageG3,
            if(x2.is_miles_0_9, if(x2.age between 25 and 34, if(x2.gender = 'female',1,0),0),0) as is_female_0_9_ageG3,

            if(x2.is_miles_0_9, if(x2.age between 35 and 44, if(x2.gender = 'male',1,0),0),0) as is_male_0_9_ageG4,
            if(x2.is_miles_0_9, if(x2.age between 35 and 44, if(x2.gender = 'female',1,0),0),0) as is_female_0_9_ageG4,

            if(x2.is_miles_0_9, if(x2.age between 45 and 54, if(x2.gender = 'male',1,0),0),0) as is_male_0_9_ageG5,
            if(x2.is_miles_0_9, if(x2.age between 45 and 54, if(x2.gender = 'female',1,0),0),0) as is_female_0_9_ageG5,

            if(x2.is_miles_0_9, if(x2.age between 55 and 64, if(x2.gender = 'male',1,0),0),0) as is_male_0_9_ageG6,
            if(x2.is_miles_0_9, if(x2.age between 55 and 64, if(x2.gender = 'female',1,0),0),0) as is_female_0_9_ageG6,

            if(x2.is_miles_0_9, if(x2.age >= 65, if(x2.gender = 'male',1,0),0),0) as is_male_0_9_ageG7,
            if(x2.is_miles_0_9, if(x2.age >= 65, if(x2.gender = 'female',1,0),0),0) as is_female_0_9_ageG7
        from
        (
            select
                if(x1.miles_logged > 40, 1, 0) as is_miles_0_9,
                x1.age as age,
                x1.gender as gender
            from 
                (
                    select
                        t1.id,
                        t2.gender,
                        DATE_FORMAT(FROM_DAYS(TO_DAYS(NOW())-TO_DAYS(t2.dob)), '%Y')+0 as age,
                        sum(t3.miles) as miles_logged
                    from
                        table1 t1
                    inner join
                        table2 t2
                        on t1.id = t2.user_id
                    inner join
                        table3 t3
                        on t2.user_id = t3.user_id
                    where
                        t1.date_created > '2012-02-29'
                    group by
                        t1.id
                ) x1
        ) x2
    )x3