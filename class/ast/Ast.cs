using System;
using System.Collections.Generic;
using System.Linq;

namespace Cursed_compiler
{
    class Ast
    {
        class Node{
            public string data;
            public string value;
            public List<Node> children;
            public Node(string data, string value){
                this.data=data;
                this.value=value;
            }
        }

        Node tree=new Node("program", null);
        public Ast(Parser parse, Dictionary<string, List<string>> tokens){
            var myStack = parse.generatedStack;
            
			String [] valuesTokens = new String[tokens.Count];
            var pretokens = tokens.Values.ToList();

            for(int i=0; i<tokens.Count; i++){
				valuesTokens[i]=pretokens[i].ToList()[2];
			}

            Stack<Node> myTree = buildTree(myStack, valuesTokens, parse.gramarG);

            tree.children=new List<Node>();
            foreach(Node n in myTree){
                tree.children.Add(n);
            }
        }

        Stack<Node> buildTree(List<string> tokens, String [] valuesTokens, String[] rules){
            Stack<Node> stack = new Stack<Node>();

            int index=0;
            foreach(string item in tokens){
                if(item!=""){
                    if(!isHead(item, rules)){
                        stack.Push(new Node(item, valuesTokens[index]));
                        index++;
                    }else{
                        var tail = myTail(item, rules, stack);
                        var childs = new List<Node>(tail);

                        for(int i=0; i<childs.Count; i++){
                            stack.Pop();
                        }
                        
                        stack.Push(new Node (item, null){children = childs});
                    }
                }
            }
            return stack;
        }

        Queue<Node> myTail(String token, String [] rules, Stack<Node> theRealStack){
            Queue<Node> miniStack = new Queue<Node>();

            String myStack="";
            for(int i=theRealStack.Count-1; i>=0; i--){
                myStack+=" "+theRealStack.ElementAt(i).data;
            }

            foreach(String rule in rules){
                String head = rule.Split(' ')[0];

                int temp = rule.IndexOf(": ")+2;
			    var thisRule = rule.Substring(temp);

                bool sub = myStack.Contains(thisRule);

                if(head == token && sub){
                    var myRule = thisRule.Split(" ").ToList();
                    // int myInd=myRule.Count-1;
                    int myStackInd=0;
                    foreach(String e in myRule){
                        miniStack.Enqueue(theRealStack.ElementAt(myStackInd));//new Node(e, theRealStack.ElementAt(myInd).value));
                        // myInd--;
                        myStackInd++;
                    }
                    break;
                }
            }
            return miniStack;
        }
        public Boolean isHead(String token, String [] rules){
            bool ans = false;
            foreach(String rule in rules){
                String head = rule.Split(' ')[0];
                if(head == token){
                    ans = true;
                    break;
                }
            }
            return ans;
        }
    }
}
