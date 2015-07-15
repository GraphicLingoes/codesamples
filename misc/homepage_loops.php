<?
class Homepage_Loops extends WP_Widget {
	function Homepage_Loops() {
		//Constructor function	
		$widget_options = array('description'=>'Controls the 3 index loops, allowing users to select which posts show up in each one based on category.');
		$this->WP_Widget("homepage_loop","Homepage Loop", $widget_options);
	}
	
	function dw_createNumSelect($id,$name,$limit,$selected){
		$output = '<select id="'.$id.'" name="'.$name.'">';
		for($i=1;$i<=$limit;$i++){
			$is_selected = $selected == $i ? "selected":"";
			$output .= '<option value="'.$i.'" '.$is_selected.'>'.$i.'</option>';
		}
		$output .= '</select>';
		return $output;
	}
	function formCreate($instance,$headline_instance, $cat_instance, $dropdown_instance,$num_post_instance){
			//Generates widget options form which you see in the widgets section in the admin
		$headline = esc_attr($instance[$headline_instance]);
		$include_cats = esc_attr($instance[$cat_instance]);
		$num_post_selected = esc_attr($instance[$num_post_instance]);
		
		?>
            <p><label for="<? echo $this->get_field_id($headline_instance); ?>">Headline:
                <input class="widefat"
                    id="<? echo $this->get_field_id($headline_instance); ?>"
                    name="<? echo $this->get_field_name($headline_instance); ?>"
                    type="text"
                    value="<? echo attribute_escape($headline); ?>" />
                </label>
            </p>
            <p>
				Categories Dropdown / How Many Posts<br />
				<? wp_dropdown_categories('orderby=name&hide_empty=0&show_option_none=Select Categories to include&id='.$dropdown_instance.'&name='.$dropdown_instance.'&taxonomy=category'); echo "&nbsp"; echo $this->dw_createNumSelect($this->get_field_id($num_post_instance),$this->get_field_name($num_post_instance),10,$num_post_selected);?> <br /><br />
                <label for="<? echo $this->get_field_id($cat_instance); ?>">Included Categories:
                	<textarea class="widefat"
                    	id="<? echo $this->get_field_id($cat_instance); ?>"
                        name="<? echo $this->get_field_name($cat_instance); ?>"
                        rows="2"
                        cols="1" style="text-align:left;"><? echo attribute_escape($include_cats); ?></textarea>
               </label>
            </p>
       <?
	}
	function form($instance){
		?>
        <em>Use drop down menus to add categories to boxes.</em>
        <?
		$this->formCreate($instance,"headline1","include_cats1","dw_main_cats1","dw_loop_posts_limit1");
		$this->formCreate($instance,"headline2","include_cats2","dw_main_cats2","dw_loop_posts_limit2");
		$this->formCreate($instance,"headline3","include_cats3","dw_main_cats3","dw_loop_posts_limit3");
		?>
            <script type="text/javascript">
					function onChangeHandler(selectName,textAreaID){
						jQuery("select[name='"+selectName+"']").change(function(){
							 // Grab selected value from drop down
							 // Grab slected text from drop down and remove inital text
							 // Check to make sure selected value is not index -1
							 // Then add value and text to text area
							 // Check to make sure value and text does not already exist
							 var selectedCat = jQuery(this).val();
							 var selectedText = jQuery("select[name='"+selectName+"'] option:selected").text().replace("Select Categories to include","");
							 if(selectedCat != -1){
								 var textArea = jQuery("#"+textAreaID+"");
								 var textAreaVal = textArea.val();
								 var textAreaArray = textAreaVal.split(',');
								 var validateSelection = jQuery.inArray(selectedCat+":"+selectedText,textAreaArray);
								 if(validateSelection == -1){
									if(textAreaVal == ""){
										textArea.append(selectedCat+":"+selectedText);
									} else {
										textArea.append(","+selectedCat+":"+selectedText);	
									}
								 } else {
									alert("Category already selected"); 
								 }
							 }
						});
					}
					// Init on change event handlers
					onChangeHandler('dw_main_cats1','<? echo $this->get_field_id('include_cats1'); ?>');
					onChangeHandler('dw_main_cats2','<? echo $this->get_field_id('include_cats2'); ?>');
					onChangeHandler('dw_main_cats3','<? echo $this->get_field_id('include_cats3'); ?>');
			</script>
        <?	
	}
	
	function update($new_instance,$old_instance){
		//Handles the submission of the options form to update the widget options
		$instance = $old_instance;
		$instance['headline1'] = strip_tags($new_instance['headline1']);
		$instance['include_cats1'] = strip_tags($new_instance['include_cats1']);
		$instance['dw_loop_posts_limit1'] = strip_tags($new_instance['dw_loop_posts_limit1']);
		$instance['headline2'] = strip_tags($new_instance['headline2']);
		$instance['include_cats2'] = strip_tags($new_instance['include_cats2']);
		$instance['dw_loop_posts_limit2'] = strip_tags($new_instance['dw_loop_posts_limit2']);
		$instance['headline3'] = strip_tags($new_instance['headline3']);
		$instance['include_cats3'] = strip_tags($new_instance['include_cats3']);
		$instance['dw_loop_posts_limit3'] = strip_tags($new_instance['dw_loop_posts_limit3']);
		
		return $instance;	
	}
	
	function extractCategories($category_string){
		$explode_cats = explode(",",$category_string);
		$cat_str_builder = "";
		foreach($explode_cats as $cat){
			$temp_cat = explode(":",$cat);
			$cat_str_builder .= $temp_cat[0].",";	
		}
		$category_str = substr($cat_str_builder, 0,-1);
		return $category_str;
	}
	
	function widget($args,$instance){
		//Handles outputting the widgets content to theme
		extract($args, EXTRA_SKIP);
		echo $before_widget;
		$categories1 = $this->extractCategories($instance['include_cats1']);
		$post_per_page = $instance['dw_loop_posts_limit1'];
		query_posts("posts_per_page=".$post_per_page."&cat=".$categories1);
		$counter = 1;
		?><div class="dw-latest-headlines">
		<div class="dw-title-block"><h1><? echo $instance['headline1']; ?></h1></div><?php
		if(have_posts()) : while (have_posts()) : the_post(); 
			if(!is_sticky()):
		?> 
			<div id="post-<?php the_id() ?>" class="dw-home-posts">
			<?php thematic_postheader();
			if($counter == 1 && has_post_thumbnail()) {
				the_post_thumbnail('homepage-thumbnail');
			} ?>
			<div class="entry-content">
				<?php the_excerpt(); ?>
				<a href="<?php the_permalink(); ?>" class="more"><?php echo more_text() ?></a>
				<?php $counter++; ?>
			</div>
		   </div><!-- .post -->
		<?php endif; ?>
		<?php endwhile; else: ?>
			<!-- No post available -->
		<?php endif; ?>
		<?php
		#####
		## Start second loop
		###
        wp_reset_query();
        $categories2 = $this->extractCategories($instance['include_cats2']);
		$post_per_page2 = $instance['dw_loop_posts_limit2'];
		query_posts("posts_per_page=".$post_per_page2."&cat=".$categories2);
        $counter = 1;
        ?>
        <div class="dw-title-block"><h1><? echo $instance['headline2']; ?></h1></div><?php
        if(have_posts()) : while (have_posts()) : the_post(); 
            if(is_sticky()):
        ?> 
            <div id="post-<?php the_id() ?>" class="dw-home-posts">
            <?php thematic_postheader();
            if($counter == 1 && has_post_thumbnail()) {
                the_post_thumbnail('homepage-thumbnail');
            } ?>
           </div><!-- .post -->
        <?php endif; ?>
        <?php endwhile; else: ?>
            <!-- No post available -->
        <?php endif; ?>
        <!-- /dw-sticky-headlines -->
         <?php
		 ######
		 ## Start 3rd loop
		 ######
        wp_reset_query();
        $categories3 = $this->extractCategories($instance['include_cats3']);
		$post_per_page3 = $instance['dw_loop_posts_limit3'];
		query_posts("posts_per_page=".$post_per_page3."&cat=".$categories3);
        $counter = 1;
        ?>
        <div class="dw-title-block"><h1><? echo $instance['headline3']; ?></h1></div><?php
        if(have_posts()) : while (have_posts()) : the_post(); 
            if(!is_sticky()):
        ?> 
            <div id="post-<?php the_id() ?>" class="dw-home-posts">
            <?php thematic_postheader();
            if($counter == 1 && has_post_thumbnail()) {
                the_post_thumbnail('homepage-thumbnail');
            } ?>
           </div><!-- .post -->
        <?php endif; ?>
        <?php endwhile; else: ?>
            <!-- No post available -->
        <?php endif; ?>
         </div><!-- /dw-latest-headlines -->
         </div>
         <?php
        wp_reset_query();
	   echo $after_widget;	
	} // end widget function
	
}

register_widget("Homepage_Loops");

?>