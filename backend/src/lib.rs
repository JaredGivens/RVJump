// Copyright 2018-2023 the Deno authors. All rights reserved. MIT license.
//!  This example shows you how to evaluate JavaScript expression and deserialize
//!  return value into a Rust object.

// NOTE:
// Here we are deserializing to `serde_json::Value` but you can
// deserialize to any other type that implementes the `Deserialize` trait.

use deno_core::FastString;
use deno_core::v8;
use serde_v8;
use deno_core::JsRuntime;
use deno_core::RuntimeOptions;
use std::ffi::CStr;
use std::ffi::CString;
use std::ffi::c_char;

#[no_mangle]
pub extern "C" fn loadAsm(instruction: *const c_char) {
  let instruction = unsafe { CString::from(CStr::from_ptr(instruction)) };

  let mut runtime = JsRuntime::new(RuntimeOptions::default());
  // let main_module = deno_core::resolve_path("Instruction.js", Path::new("C:/Users/abhi-/source/UnityProjects/RVJump/backend/encoder"));

  let instruction_setup = String::from(include_str!("../encoder/Instruction.js"));
  eval(&mut runtime, instruction_setup.into()).expect("Eval failed");

  let binding = instruction.into_string().unwrap();
  let instructions: Vec<&str> = binding.split("\n").collect();

  let mut instr_memory = Vec::new();
  
  for instr in instructions {
    let wrapped_instr = format!("\ninstr = new Instruction('{}'); instr.bin", instr);
    let res = eval(&mut runtime, wrapped_instr.into()).expect("Eval failed");
    let result_string = res.as_str().unwrap();
    let instr_word = u32::from_str_radix(result_string, 2).unwrap();
    instr_memory.extend(instr_word.to_le_bytes());
    dbg!(instr_word);
  }
}

fn eval(
  context: &mut JsRuntime,
  code: FastString,
) -> Result<serde_json::Value, String> {
  let res = context.execute_script("<anon>", code);
  match res {
    Ok(global) => {
      let scope = &mut context.handle_scope();
      let local = v8::Local::new(scope, global);
      // Deserialize a `v8` object into a Rust type using `serde_v8`,
      // in this case deserialize to a JSON `Value`.
      let deserialized_value =
        serde_v8::from_v8::<serde_json::Value>(scope, local);

      match deserialized_value {
        Ok(value) => Ok(value),
        Err(err) => Err(format!("Cannot deserialize value: {err:?}")),
      }
    }
    Err(err) => Err(format!("Evaling error: {err:?}")),
  }
}

#[cfg(test)]
mod tests {
    use std::ffi::CString;
    use crate::loadAsm;

  #[test]
  fn test_add() {
    loadAsm(CString::new("add x1,x2,x3\nsub x1,x2,x3\naddi x1,x2,3").unwrap().as_ptr());
  }
}
